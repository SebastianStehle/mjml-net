using System.Text;
using System.Xml;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IHtmlRenderer, IElementHtmlRenderer, IChildRenderer
    {
        private readonly RenderStack<StringBuilder> buffers = new RenderStack<StringBuilder>();
        private bool currentSelfClosed;
        private int numClasses;
        private int numStyles;
        private int currentIntend;
        private bool currentlyWriting;

        public INode Node
        {
            get => this;
        }

        public XmlReader Reader
        {
            get => contextStack.Current!.Reader!;
        }

        public GlobalData GlobalData
        {
            get => globalData;
        }

        private StringBuilder Buffer
        {
            get => buffers.Current!;
        }

        private void ClearRenderData()
        {
            buffers.Clear();
            contextStack.Clear();
            currentSelfClosed = false;
            currentIntend = 0;
        }

        public void BufferStart()
        {
            Flush();

            // Rent a buffer to avoid memory allocations.
            buffers.Push(ObjectPools.StringBuilder.Get());
        }

        public string BufferFlush()
        {
            if (Buffer == null)
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

            WriteLineStart();

            Buffer.Append('<');
            Buffer.Append(elementName);

            currentlyWriting = true;
            numStyles = 0;
            numClasses = 0;
            currentSelfClosed = selfClosed;

            return this;
        }

        public IElementHtmlRenderer Attr(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
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

        public IElementClassWriter Class(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            if (numClasses == 0)
            {
                Buffer.Append(" class=\"");
            }
            else
            {
                Buffer.Append(' ');
            }

            Buffer.Append(value);

            numClasses++;

            return this;
        }

        public IElementStyleWriter Style(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            if (numClasses > 0)
            {
                Buffer.Append("\" ");

                numClasses = 0;
            }

            if (numStyles == 0)
            {
                Buffer.Append(" style=\"");
            }

            Buffer.Append(name);
            Buffer.Append(':');
            Buffer.Append(value);
            Buffer.Append(';');

            numStyles++;

            return this;
        }

        public void ElementEnd(string elementName)
        {
            Flush();

            currentIntend--;

            WriteLineStart();

            Buffer.Append("</");
            Buffer.Append(elementName);
            Buffer.Append('>');

            WriteLineEnd();
        }

        public void Content(string? value)
        {
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
                Buffer.Append(value);
            }

            WriteLineEnd();
        }

        public void Plain(string? value, bool appendLine = true)
        {
            if (Buffer == null)
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
                Buffer.Append(value);
            }

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

        private void WriteMinified(string value)
        {
            Buffer.EnsureCapacity(Buffer.Length + value.Length);

            // We could go over the chars but it is much faster to writer to the buffer in batches. Therefore we create a span from newline to newline.
            var span = value.AsSpan();

            for (int i = 0, j = 0; i < value.Length; i++, j++)
            {
                if (value[i] == '\n')
                {
                    Buffer.Append(span[..j].Trim());

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
            if (options.Beautify)
            {
                Buffer.AppendLine();
            }
        }

        private void WriteLineStart()
        {
            if (options.Beautify)
            {
                for (var i = 0; i < currentIntend; i++)
                {
                    Buffer.Append("  ");
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
            if (!currentlyWriting)
            {
                return;
            }

            if (numClasses > 0 || numStyles > 0)
            {
                Buffer.Append('\"');
            }

            if (currentSelfClosed)
            {
                Buffer.Append("/>");
            }
            else
            {
                currentIntend++;
                Buffer.Append('>');
            }

            WriteLineEnd();

            currentlyWriting = false;
            currentSelfClosed = false;
        }
    }
}
