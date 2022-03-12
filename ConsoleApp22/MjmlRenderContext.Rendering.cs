using System.Text;

namespace ConsoleApp22
{
    internal partial class MjmlRenderContext : IHtmlRenderer, IElementHtmlRenderer
    {
        private readonly Dictionary<string, string> currentRenderStyles = new Dictionary<string, string>(10);
        private readonly Dictionary<string, string> currentRenderAttributes = new Dictionary<string, string>(10);
        private readonly StringBuilder sb = new StringBuilder();
        private string? currentRenderElement;

        public override string ToString()
        {
            return sb.ToString();
        }

        public IElementHtmlRenderer StartElement(string elementName)
        {
            currentRenderElement = elementName;

            currentRenderStyles.Clear();
            currentRenderAttributes.Clear();

            return this;
        }

        public void RenderChildren()
        {
            Read();
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

        public IElementHtmlRenderer Style(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            currentRenderStyles[name] = value;

            return this;
        }

        public IElementHtmlRenderer Done()
        {
            currentRenderStyles.Clear();
            currentRenderAttributes.Clear();

            WriteLineStart(2);

            sb.Append('<');
            sb.Append(currentRenderElement);

            if (currentRenderAttributes.Count > 0)
            {
                foreach (var (key, value) in currentRenderAttributes)
                {
                    sb.Append(' ');
                    sb.Append(key);
                    sb.Append("=\"");
                    sb.Append(value);
                    sb.Append('"');
                }
            }

            if (currentRenderStyles.Count > 0)
            {
                sb.Append("style=\"");

                foreach (var (key, value) in currentRenderStyles)
                {
                    sb.Append(key);
                    sb.Append(':');
                    sb.Append(' ');
                    sb.Append(value);
                    sb.Append(';');
                }

                sb.Append('"');
            }

            sb.Append('>');

            WriteLineEnd();

            return this;
        }

        public void EndElement(string elementName)
        {
            WriteLineStart(0);

            sb.Append("</");
            sb.Append(elementName);
            sb.Append('>');

            WriteLineEnd();

            intend -= 2;
        }

        public void Content(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            WriteLineStart(2);

            sb.Append(value);

            WriteLineEnd();
        }

        public void Plain(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            WriteLineStart(0);

            sb.Append(value);

            WriteLineEnd();
        }

        private void WriteLineEnd()
        {
            sb.AppendLine();
        }

        private void WriteLineStart(int increment)
        {
            intend += increment;

            for (var i = 0; i < intend; i++)
            {
                sb.Append(' ');
            }
        }

        public IElementHtmlRenderer Class(string? value)
        {
            throw new NotImplementedException();
        }
    }
}
