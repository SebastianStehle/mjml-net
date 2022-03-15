using System.Text;
using System.Xml;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IHtmlRenderer, IElementHtmlRenderer, IChildRenderer
    {
        private readonly Dictionary<string, string> currentRenderStyles = new Dictionary<string, string>(10);
        private readonly Dictionary<string, string> currentRenderAttributes = new Dictionary<string, string>(10);
        private readonly HashSet<string> currentRenderClasses = new HashSet<string>(10);
        private readonly RenderStack<StringBuilder> buffers = new RenderStack<StringBuilder>();
        private string? currentRenderElement;
        private bool currentRenderElementSelfClosed;
        private int intend;

        public XmlReader Reader => contextStack.Current!.Reader;

        public INode Node => this;

        public GlobalData GlobalData => globalData;

        private void ClearRenderData()
        {
            buffers.Clear();
            contextStack.Clear();
            currentRenderAttributes.Clear();
            currentRenderClasses.Clear();
            currentRenderElement = null;
            currentRenderElementSelfClosed = false;
            currentRenderStyles.Clear();
            intend = 0;
        }

        public void BufferStart()
        {
            Flush();

            // Rent a buffer to avoid memory allocations.
            buffers.Push(ObjectPools.StringBuilder.Get());
        }

        public string BufferFlush()
        {
            if (buffers.Current == null)
            {
                return string.Empty;
            }

            Flush();

            var currentBuffer = buffers.Pop();
            try
            {
                return currentBuffer?.ToString() ?? string.Empty;
            }
            finally
            {
                if (currentBuffer != null)
                {
                    ObjectPools.StringBuilder.Return(currentBuffer);
                }
            }
        }

        public void RenderHelpers(HelperTarget target)
        {
            foreach (var helper in renderer.Helpers)
            {
                helper.Render(this, target, globalData);
            }
        }

        public IElementHtmlRenderer ElementStart(string elementName, bool selfClosed = false)
        {
            Flush();

            if (string.IsNullOrWhiteSpace(elementName))
            {
                return this;
            }

            currentRenderElement = elementName;
            currentRenderElementSelfClosed = selfClosed;

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
            if (buffers.Current == null)
            {
                return;
            }

            Flush();

            intend--;

            WriteLineStart();

            buffers.Current.Append("</");
            buffers.Current.Append(elementName);
            buffers.Current.Append('>');

            WriteLineEnd();
        }

        public void Content(string? value)
        {
            if (buffers.Current == null)
            {
                return;
            }

            Flush();

            if (value == null)
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
                buffers.Current.Append(value);
            }

            WriteLineEnd();
        }

        public void Plain(string? value, bool appendLine = true)
        {
            if (buffers.Current == null)
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
                buffers.Current.Append(value);
            }

            if (appendLine)
            {
                WriteLineEnd();
            }
        }

        private void WriteIntended(string value)
        {
            buffers.Current!.EnsureCapacity(buffers.Current.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    buffers.Current.Append(span[.. (j + 1)]);

                    WriteLineStart();

                    // Start the span after the newline.
                    span = span[(j + 1)..];
                    j = -1;
                }
            }

            buffers.Current.Append(span);
        }

        private void WriteMinified(string value)
        {
            buffers.Current!.EnsureCapacity(buffers.Current.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    buffers.Current.Append(span[..j].Trim());

                    WriteLineStart();

                    // Start the span after the newline.
                    span = span[(j + 1)..];
                    j = -1;
                }
            }

            buffers.Current.Append(span);
        }

        private void WriteLineEnd()
        {
            if (options.Beautify)
            {
                buffers.Current!.AppendLine();
            }
        }

        private void WriteLineStart()
        {
            if (options.Beautify)
            {
                for (var i = 0; i < intend; i++)
                {
                    buffers.Current!.Append("  ");
                }
            }
        }

        public void RenderChildren()
        {
            RenderChildren(default);
        }

        public void RenderChildren(ChildOptions options)
        {
            var reader = contextStack.Current!.Reader;

            if (options.RawXML)
            {
                var inner = reader.ReadInnerXml().Trim();

                Content(inner);
            }
            else
            {
                reader.Read();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ReadElement(reader.Name, reader, options);
                            break;
                    }
                }
            }
        }

        void IChildRenderer.Render()
        {
            currentComponent!.Render(this, this);
        }

        private void Flush()
        {
            if (currentRenderElement == null || buffers.Current == null)
            {
                return;
            }

            WriteLineStart();

            buffers.Current.Append('<');
            buffers.Current.Append(currentRenderElement);

            if (currentRenderAttributes.Count > 0)
            {
                foreach (var (key, value) in currentRenderAttributes)
                {
                    buffers.Current.Append(' ');
                    buffers.Current.Append(key);
                    buffers.Current.Append("=\"");
                    buffers.Current.Append(value);
                    buffers.Current.Append('"');
                }
            }

            if (currentRenderStyles.Count > 0)
            {
                buffers.Current.Append(" style=\"");

                foreach (var (key, value) in currentRenderStyles)
                {
                    buffers.Current.Append(key);
                    buffers.Current.Append(':');
                    buffers.Current.Append(value);
                    buffers.Current.Append(';');
                }

                buffers.Current.Append('"');
            }

            if (currentRenderClasses.Count > 0)
            {
                buffers.Current.Append(" class=\"");

                var index = 0;
                foreach (var value in currentRenderClasses)
                {
                    buffers.Current.Append(value);

                    if (index < currentRenderClasses.Count - 1)
                    {
                        buffers.Current.Append(' ');
                    }

                    index++;
                }

                buffers.Current.Append('"');
            }

            buffers.Current.Append('>');

            WriteLineEnd();

            if (!currentRenderElementSelfClosed)
            {
                intend++;
            }

            currentRenderElement = null;
            currentRenderElementSelfClosed = false;
            currentRenderClasses.Clear();
            currentRenderStyles.Clear();
            currentRenderAttributes.Clear();
        }
    }
}
