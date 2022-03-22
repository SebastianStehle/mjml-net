using System.Runtime.CompilerServices;
using System.Text;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IHtmlRenderer, IHtmlAttrRenderer
    {
        private readonly RenderStack<StringBuilder> buffers = new RenderStack<StringBuilder>();
        private readonly HashSet<string> analyzedFonts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool elementSelfClosed;
        private bool elementStarted;
        private int numClasses;
        private int numStyles;
        private int indent;
        private string? pendingConditionalStart;
        private string? pendingConditionalEnd;
        private int conditionalDepth;

        public StringBuilder Buffer
        {
            get => buffers.Current!;
        }

        public StringBuilder StringBuilder
        {
            get => buffers.Current!;
        }

        private void ClearRenderData()
        {
            analyzedFonts.Clear();
            buffers.Clear();
            elementStarted = false;
            elementSelfClosed = false;
            indent = 0;
        }

        public void StartBuffer()
        {
            buffers.Push(ObjectPools.StringBuilder.Get());
        }

        public StringBuilder? EndBuffer()
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            var currentBuffer = buffers.Pop();

            return currentBuffer;
        }

        public void RenderHelpers(HelperTarget target)
        {
            foreach (var helper in mjmlRenderer.Helpers)
            {
                helper.Render(this, target, context);
            }
        }

        public IHtmlAttrRenderer StartElement(string elementName, bool close = false)
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            if (string.IsNullOrEmpty(elementName))
            {
                return this;
            }

            WriteLineStart();

            Buffer.Append('<');
            Buffer.Append(elementName);

            elementSelfClosed = close;
            elementStarted = true;
            numClasses = 0;
            numStyles = 0;

            return this;
        }

        public IHtmlAttrRenderer Attr(string name, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            DetectFontFamily(name, value);

            StartAttr(name);

            Buffer.Append(value);

            EndAttr();

            return this;
        }

        public IHtmlAttrRenderer Attr(string name, [InterpolatedStringHandlerArgument("", "name")] ref AttrInterpolatedStringHandler value)
        {
            // Starting the attribute is done by the interpolation handler.
            EndAttr();

            return this;
        }

        public void StartAttr(string name)
        {
            Buffer.Append(' ');
            Buffer.Append(name);
            Buffer.Append("=\"");
        }

        private void EndAttr()
        {
            Buffer.Append('"');
        }

        public IHtmlClassRenderer Class(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            StartClass();

            Buffer.Append(value);

            EndClass();

            return this;
        }

        public IHtmlClassRenderer Class([InterpolatedStringHandlerArgument("")] ref ClassNameInterpolatedStringHandler value)
        {
            // Starting the class is done by the interpolation handler.
            EndClass();

            return this;
        }

        public void StartClass()
        {
            if (numClasses == 0)
            {
                // Open the class attribute.
                Buffer.Append(" class=\"");
            }
            else
            {
                Buffer.Append(' ');
            }
        }

        private void EndClass()
        {
            numClasses++;
        }

        public IHtmlStyleRenderer Style(string name, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            DetectFontFamily(name, value);

            StartStyle(name);

            Buffer.Append(value);

            EndStyle();

            return this;
        }

        public IHtmlStyleRenderer Style(string name, [InterpolatedStringHandlerArgument("", "name")] ref StyleInterpolatedStringHandler value)
        {
            // Ending the style is done by the interpolation handler.
            EndStyle();

            return this;
        }

        public void StartStyle(string name)
        {
            if (numClasses > 0)
            {
                // Close the open class attribute.
                Buffer.Append("\" ");

                // Reset the number of classes so we do not close it again in the flush.
                numClasses = 0;
            }

            if (numStyles == 0)
            {
                // Open the styles attribute.
                Buffer.Append(" style=\"");
            }

            Buffer.Append(name);
            Buffer.Append(':');
        }

        private void EndStyle()
        {
            Buffer.Append(';');

            numStyles++;
        }

        public void EndElement(string elementName)
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            indent--;

            WriteLineStart();

            Buffer.Append("</");
            Buffer.Append(elementName);
            Buffer.Append('>');

            WriteLineEnd();
        }

        private void FlushElement()
        {
            if (!elementStarted)
            {
                return;
            }

            if (numClasses > 0 || numStyles > 0)
            {
                // Close the open class or style attribute.
                Buffer.Append('\"');
            }

            if (elementSelfClosed)
            {
                Buffer.Append("/>");
            }
            else
            {
                indent++;
                Buffer.Append('>');
            }

            WriteLineEnd();

            elementStarted = false;
            elementSelfClosed = false;
        }

        public void Content([InterpolatedStringHandlerArgument("")] ref TextInterpolatedStringHandler value)
        {
            WriteLineEnd();
        }

        public void Content(string? value)
        {
            FlushElement();

            if (value == null)
            {
                return;
            }

            TextCore(value);
        }

        private void TextCore(string value)
        {
            WriteLineStart();

            if (mjmlOptions.Beautify)
            {
                WriteIntended(value);
            }
            else
            {
                Buffer.Append(value);
            }

            WriteLineEnd();
        }

        public void StartText()
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            WriteLineStart();
        }

        public void Plain(StringBuilder? value, bool newLine = true)
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            if (value?.Length == 0)
            {
                return;
            }

            Buffer.Append(value);

            if (newLine)
            {
                WriteLineEnd();
            }
        }

        public void Plain(ReadOnlySpan<char> value, bool newLine = true)
        {
            FlushElement();
            FlushConditionalStart();
            FlushConditionalEnd();

            if (value.Length == 0)
            {
                return;
            }

            Buffer.Append(value);

            if (newLine)
            {
                WriteLineEnd();
            }
        }

        private void WriteIntended(string value)
        {
            Buffer.EnsureCapacity(Buffer.Length + value.Length);

            // We could go over the chars but it is much faster to write to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    Buffer.Append(span[.. (j + 1)]);

                    WriteLineStart();

                    // Start the span after the newline.
                    span = span[(j + 1)..];
                    j = -1;
                }
            }

            Buffer.Append(span);
        }

        private void WriteLineEnd()
        {
            if (mjmlOptions.Beautify)
            {
                Buffer.AppendLine();
            }
        }

        private void WriteLineStart()
        {
            if (mjmlOptions.Beautify)
            {
                for (var i = 0; i < indent; i++)
                {
                    Buffer.Append("  ");
                }
            }
        }

        public void StartConditional(string content)
        {
            FlushElement();

            conditionalDepth++;

            if (pendingConditionalStart != null || pendingConditionalEnd != null || conditionalDepth > 1)
            {
                pendingConditionalEnd = null;
                return;
            }

            pendingConditionalStart = content;
        }

        public void EndConditional(string content)
        {
            FlushElement();

            conditionalDepth--;

            if (pendingConditionalStart != null || conditionalDepth > 0)
            {
                pendingConditionalEnd = content;
                return;
            }

            pendingConditionalEnd = content;
        }

        private void FlushConditionalEnd()
        {
            if (pendingConditionalEnd == null)
            {
                return;
            }

            TextCore(pendingConditionalEnd);

            pendingConditionalEnd = null;
        }

        private void FlushConditionalStart()
        {
            if (pendingConditionalStart == null)
            {
                return;
            }

            TextCore(pendingConditionalStart);

            pendingConditionalStart = null;
        }

        private void DetectFontFamily(string name, string value)
        {
            if (name != "font-family")
            {
                return;
            }

            if (!analyzedFonts.Add(value))
            {
                return;
            }

            var hasMultipleFonts = value.Contains(',', StringComparison.OrdinalIgnoreCase);

            if (hasMultipleFonts)
            {
                // If we have multiple fonts it is faster than a string.Split, because we can avoid allocations.
                foreach (var (key, font) in mjmlOptions.Fonts)
                {
                    if (value.Contains(key, StringComparison.OrdinalIgnoreCase))
                    {
                        context.SetGlobalData(key, font, true);
                    }
                }
            }
            else
            {
                // Fast track for a single font.
                if (mjmlOptions.Fonts.TryGetValue(value, out var font))
                {
                    context.SetGlobalData(value, font, true);
                }
            }
        }
    }
}
