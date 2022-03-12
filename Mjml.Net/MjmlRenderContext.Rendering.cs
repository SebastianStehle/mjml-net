using System.Text;
using System.Xml;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IHtmlRenderer, IElementHtmlRenderer
    {
        private readonly Dictionary<string, string> currentRenderStyles = new Dictionary<string, string>(10);
        private readonly Dictionary<string, string> currentRenderAttributes = new Dictionary<string, string>(10);
        private readonly HashSet<string> currentRenderClasses = new HashSet<string>(10);
        private readonly Stack<StringBuilder> bufferStack = new Stack<StringBuilder>(2);
        private StringBuilder? buffer;
        private string? currentRenderElement;
        private int intend;

        public XmlReader Reader => reader;


        public void BufferStart()
        {
            Flush();

            // Rent a buffer to avoid memory allocations.
            buffer = ObjectPools.StringBuilder.Get();

            // Maintain a stack of buffers to get access to the previous pool when this one is flushed.
            bufferStack.Push(buffer);
        }

        public string BufferFlush()
        {
            if (buffer == null)
            {
                return string.Empty;
            }

            Flush();

            var currentPool = bufferStack.Pop();
            try
            {
                if (bufferStack.Count > 0)
                {
                    // Set back the last buffer for fast access.
                    buffer = bufferStack.Peek();
                }
                else
                {
                    buffer = null;
                }

                return currentPool.ToString();
            }
            finally
            {
                ObjectPools.StringBuilder.Return(currentPool);
            }
        }

        public void RenderHelpers(HelperTarget target)
        {
            foreach (var helper in renderer.Helpers)
            {
                helper.Render(this, target, globalData);
            }
        }

        public IElementHtmlRenderer ElementStart(string elementName)
        {
            Flush();

            if (string.IsNullOrWhiteSpace(elementName))
            {
                return this;
            }

            currentRenderElement = elementName;

            return this;
        }

        public IElementHtmlRenderer Attr(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            currentRenderAttributes[name] = value;

            return this;
        }

        public IElementHtmlRenderer Class(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            currentRenderClasses.Add(value);

            return this;
        }

        public IElementHtmlRenderer Style(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            currentRenderStyles[name] = value;

            return this;
        }

        public void ElementEnd(string elementName)
        {
            if (buffer == null)
            {
                return;
            }

            Flush();

            intend--;

            WriteLineStart();

            buffer.Append("</");
            buffer.Append(elementName);
            buffer.Append('>');

            WriteLineEnd();
        }

        public void Content(string? value)
        {
            if (buffer == null)
            {
                return;
            }

            Flush();

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            WriteLineStart();

            if (options.Beautify)
            {
                WriteIntended(value);
            }
            else if (options.Minify)
            {
                WriteMinified(value);
            }
            else
            {
                buffer.Append(value);
            }

            WriteLineEnd();
        }

        public void Plain(string? value, bool appendLine = true)
        {
            if (buffer == null)
            {
                return;
            }

            Flush();

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (options.Minify)
            {
                WriteMinified(value);
            }
            else
            {
                buffer.Append(value);
            }

            if (appendLine)
            {
                WriteLineEnd();
            }
        }

        private void WriteIntended(string value)
        {
            buffer!.EnsureCapacity(buffer.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    buffer.Append(span[..(j + 1)]);

                    WriteLineStart();

                    // Start the span after the newline.
                    span = span[(j + 1)..];
                    j = -1;
                }
            }

            buffer.Append(span);
        }

        private void WriteMinified(string value)
        {
            buffer!.EnsureCapacity(buffer.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    buffer.Append(span[..j].Trim());

                    WriteLineStart();

                    // Start the span after the newline.
                    span = span[(j + 1)..];
                    j = -1;
                }
            }

            buffer.Append(span);
        }

        private void WriteLineEnd()
        {
            if (options.Beautify)
            {
                buffer!.AppendLine();
            }
        }

        private void WriteLineStart()
        {
            if (options.Beautify)
            {
                for (var i = 0; i < intend; i++)
                {
                    buffer!.Append("  ");
                }
            }
        }

        public void RenderChildren()
        {
            RenderChildren(default);
        }

        public void RenderChildren<T>(ChildOptions<T> options)
        {
            Read();
        }

        private void Flush()
        {
            if (currentRenderElement == null || buffer == null)
            {
                return;
            }

            WriteLineStart();

            buffer.Append('<');
            buffer.Append(currentRenderElement);

            if (currentRenderAttributes.Count > 0)
            {
                foreach (var (key, value) in currentRenderAttributes)
                {
                    buffer.Append(' ');
                    buffer.Append(key);
                    buffer.Append("=\"");
                    buffer.Append(value);
                    buffer.Append('"');
                }
            }

            if (currentRenderStyles.Count > 0)
            {
                buffer.Append(" style=\"");

                var index = 0;
                foreach (var (key, value) in currentRenderStyles)
                {
                    buffer.Append(key);
                    buffer.Append(':');
                    buffer.Append(' ');
                    buffer.Append(value);

                    if (index < currentRenderStyles.Count - 1)
                    {
                        buffer.Append("; ");
                    }

                    index++;
                }

                buffer.Append('"');
            }

            if (currentRenderClasses.Count > 0)
            {
                buffer.Append(" class=\"");

                var index = 0;
                foreach (var value in currentRenderClasses)
                {
                    buffer.Append(value);

                    if (index < currentRenderClasses.Count - 1)
                    {
                        buffer.Append(" ");
                    }

                    index++;
                }

                buffer.Append('"');
            }

            buffer.Append('>');

            WriteLineEnd();

            currentRenderElement = null;
            currentRenderClasses.Clear();
            currentRenderStyles.Clear();
            currentRenderAttributes.Clear();

            intend++;
        }
    }
}
