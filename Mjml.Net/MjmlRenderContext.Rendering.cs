﻿using System.Text;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IHtmlRenderer, IElementHtmlWriter
    {
        private readonly RenderStack<StringBuilder> buffers = new RenderStack<StringBuilder>();
        private bool elementSelfClosed;
        private bool elementStarted;
        private int numClasses;
        private int numStyles;
        private int indent;

        private StringBuilder Buffer
        {
            get => buffers.Current!;
        }

        private void ClearRenderData()
        {
            buffers.Clear();
            elementStarted = false;
            elementSelfClosed = false;
            indent = 0;
        }

        public void BufferStart()
        {
            Flush();

            buffers.Push(ObjectPools.StringBuilder.Get());
        }

        public StringBuilder? BufferFlush()
        {
            Flush();

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

        public IElementHtmlWriter StartElement(string elementName, bool selfClosed = false)
        {
            Flush();

            if (string.IsNullOrEmpty(elementName))
            {
                return this;
            }

            WriteLineStart();

            Buffer.Append('<');
            Buffer.Append(elementName);

            elementSelfClosed = selfClosed;
            elementStarted = true;
            numClasses = 0;
            numStyles = 0;

            return this;
        }

        public IElementHtmlWriter Attr(string name, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            Buffer.Append(' ');
            Buffer.Append(name);
            Buffer.Append("=\"");
            Buffer.Append(value);
            Buffer.Append('"');

            return this;
        }

        public IElementHtmlWriter Attr(string name, string? value1, string? value2)
        {
            if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
            {
                return this;
            }

            Buffer.Append(' ');
            Buffer.Append(name);
            Buffer.Append("=\"");
            Buffer.Append(value1);
            Buffer.Append(", ");
            Buffer.Append(value2);
            Buffer.Append('"');

            return this;
        }

        public IElementHtmlWriter Attr(string name, double value)
        {
            if (double.IsNaN(value))
            {
                return this;
            }

            Buffer.Append(' ');
            Buffer.Append(name);
            Buffer.Append("=\"");
            Buffer.Append(value);
            Buffer.Append('"');

            return this;
        }

        public IElementHtmlWriter Attr(string name, double value, string unit)
        {
            if (double.IsNaN(value))
            {
                return this;
            }

            Buffer.Append(' ');
            Buffer.Append(name);
            Buffer.Append("=\"");
            Buffer.Append(value);
            Buffer.Append(unit);
            Buffer.Append('"');

            return this;
        }

        public IElementClassWriter Class(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            SwitchToClasses();

            Buffer.Append(value);

            numClasses++;

            return this;
        }

        public IElementClassWriter Class(ReadOnlySpan<char> value, string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
            {
                return this;
            }

            SwitchToClasses();

            Buffer.Append(value);
            Buffer.Append('-');
            Buffer.Append(suffix);

            numClasses++;

            return this;
        }

        private void SwitchToClasses()
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

        public IElementStyleWriter Style(string name, string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return this;
            }

            DetectFontFamily(name, value);

            SwitchToStyles();

            Buffer.Append(name);
            Buffer.Append(':');
            Buffer.Append(value);
            Buffer.Append(';');

            numStyles++;

            return this;
        }

        public IElementStyleWriter Style(string name, double value, string unit)
        {
            if (string.IsNullOrEmpty(unit) || double.IsNaN(value))
            {
                return this;
            }

            SwitchToStyles();

            Buffer.Append(name);
            Buffer.Append(':');
            Buffer.Append(value);
            Buffer.Append(unit);
            Buffer.Append(';');

            numStyles++;

            return this;
        }

        private void SwitchToStyles()
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
        }

        public void EndElement(string elementName)
        {
            Flush();

            indent--;

            WriteLineStart();

            Buffer.Append("</");
            Buffer.Append(elementName);
            Buffer.Append('>');

            WriteLineEnd();
        }

        public void Content(string? value, bool appendLine = true)
        {
            Flush();

            if (value == null)
            {
                return;
            }

            WriteLineStart();

            if (mjmlOptions.Beautify)
            {
                WriteIntended(value);
            }
            else
            {
                Buffer.Append(value);
            }

            if (appendLine)
            {
                WriteLineEnd();
            }
        }

        public void Plain(string? value, bool appendLine = true)
        {
            Plain(value.AsSpan(), appendLine);
        }

        public void Plain(StringBuilder? value, bool appendLine = true)
        {
            Flush();

            if (value?.Length == 0)
            {
                return;
            }

            Buffer.Append(value);

            if (appendLine)
            {
                WriteLineEnd();
            }
        }

        public void Plain(ReadOnlySpan<char> value, bool appendLine = true)
        {
            Flush();

            if (value.Length == 0)
            {
                return;
            }

            Buffer.Append(value);

            if (appendLine)
            {
                WriteLineEnd();
            }
        }

        private void WriteIntended(string value)
        {
            Buffer.EnsureCapacity(Buffer.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
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

        private void Flush()
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

        private void DetectFontFamily(string name, string value)
        {
            if (name != "font-family")
            {
                return;
            }

            if (value.Contains(',', StringComparison.OrdinalIgnoreCase))
            {
                // If we have multiple fonts it is faster than a string.Split, because we can avoid allocations.
                foreach (var (key, font) in mjmlOptions.Fonts)
                {
                    if (value.Contains(key, StringComparison.OrdinalIgnoreCase))
                    {
                        context.SetGlobalData(key, font);
                    }
                }
            }
            else
            {
                // Fast track for a single font.
                if (mjmlOptions.Fonts.TryGetValue(value, out var font))
                {
                    context.SetGlobalData(value, font);
                }
            }
        }
    }
}
