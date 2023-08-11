using System.Buffers;
using System.Text;
using HtmlPerformanceKit;
using HtmlReaderImpl = HtmlPerformanceKit.HtmlReader;

namespace Mjml.Net.Internal
{
    internal class HtmlReaderWrapper : IHtmlReader
    {
        private readonly HtmlReaderImpl inner;
        private readonly byte[]? buffer;

        public int LineNumber => inner.LineNumber;

        public int LinePosition => inner.LinePosition;

        public int AttributeCount => inner.AttributeCount;

        public string Name => inner.Name;

        public string Text => inner.Text;

        public bool SelfClosingElement => inner.SelfClosingElement;

        public HtmlTokenKind TokenKind => inner.TokenKind;

        public HtmlReaderWrapper(HtmlReaderImpl inner)
        {
            this.inner = inner;
        }

        public HtmlReaderWrapper(string input)
        {
            var inputSize = Encoding.UTF8.GetByteCount(input);
            var inputBuffer = ArrayPool<byte>.Shared.Rent(inputSize);

            Encoding.UTF8.GetBytes(input.AsSpan(), inputBuffer.AsSpan());

            inner = new HtmlReaderImpl(new MemoryStream(inputBuffer, 0, inputSize));

            buffer = inputBuffer;
        }

        public virtual void Dispose()
        {
            if (buffer != null)
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public string GetAttribute(string name)
        {
            return inner.GetAttribute(name);
        }

        public string GetAttribute(int index)
        {
            return inner.GetAttribute(index);
        }

        public string GetAttributeName(int index)
        {
            return inner.GetAttributeName(index);
        }

        public virtual bool Read()
        {
            return inner.Read();
        }

        public IHtmlReader ReadSubtree()
        {
            return new SubtreeReader(inner);
        }

        public string ReadOuterHtml()
        {
            var stringBuilder = DefaultPools.StringBuilders.Get();
            try
            {
                var subTree = ReadSubtree();

                do
                {
                    switch (TokenKind)
                    {
                        case HtmlTokenKind.Text:
                            stringBuilder.Append(subTree.Text);
                            break;
                        case HtmlTokenKind.Tag:
                            stringBuilder.Append('<');
                            stringBuilder.Append(subTree.Name);

                            for (var i = 0; i < subTree.AttributeCount; i++)
                            {
                                var attributeName = subTree.GetAttributeName(i);
                                var attributeValue = subTree.GetAttribute(i);

                                stringBuilder.Append(' ');
                                stringBuilder.Append(attributeName);
                                stringBuilder.Append('=');
                                stringBuilder.Append('"');
                                stringBuilder.Append(attributeValue);
                                stringBuilder.Append('"');
                            }

                            if (subTree.SelfClosingElement)
                            {
                                stringBuilder.Append("/>");
                            }
                            else
                            {
                                stringBuilder.Append('>');
                            }
                            break;
                        case HtmlTokenKind.Comment:
                            stringBuilder.Append("<!-- ");
                            stringBuilder.Append(subTree.Text);
                            stringBuilder.Append(" -->");
                            break;
                        case HtmlTokenKind.EndTag:
                            stringBuilder.Append("</");
                            stringBuilder.Append(subTree.Name);
                            stringBuilder.Append('>');
                            break;
                    }
                }
                while (subTree.Read());

                return stringBuilder.ToString();
            }
            finally
            {
                DefaultPools.StringBuilders.Return(stringBuilder);
            }
        }
    }
}
