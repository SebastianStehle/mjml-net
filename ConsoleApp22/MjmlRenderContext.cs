using System.Text;
using System.Xml;

namespace ConsoleApp22
{
    internal class MjmlRenderContext : INode, IHtmlRenderer, IElementHtmlRenderer
    {
        private readonly MjmlRenderer renderer;
        private readonly XmlReader reader;
        private readonly Dictionary<string, string> currentAttributes = new Dictionary<string, string>();
        private readonly Dictionary<string, string> currentRenderStyles = new Dictionary<string, string>();
        private readonly Dictionary<string, string> currentRenderAttributes = new Dictionary<string, string>();
        private readonly Dictionary<string, object?> context = new Dictionary<string, object?>();
        private readonly StringBuilder sb = new StringBuilder();
        private IComponent? currentComponent;
        private string? previousElement;
        private string? currentRenderElement;
        private string? currentText;
        private string? currentElement;
        private int intend = -2;

        public MjmlRenderContext(MjmlRenderer renderer, XmlReader reader)
        {
            this.renderer = renderer;
            this.reader = reader;
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public void Read()
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        ReadAttribute(reader.Name, reader.Value);
                        break;
                    case XmlNodeType.Element:
                        if (previousElement != null)
                        {
                            ReadElement(reader.Name);
                            return;
                        }
                        else
                        {
                            ReadElement(reader.Name);
                            break;
                        }
                    case XmlNodeType.Text:
                        ReadText(reader.Value);
                        return;
                    case XmlNodeType.EndElement:
                        ReadEndElement();
                        break;
                }
            }
        }

        private void ReadEndElement()
        {
            currentElement = previousElement;

            ReadPreviousElement();
        }

        private void ReadText(string value)
        {
            currentText = value;
            currentElement = previousElement;

            ReadPreviousElement();
        }

        private void ReadElement(string name)
        {
            currentElement = previousElement;

            previousElement = name;

            ReadPreviousElement();

            // Clear the attributes because we start a new element.
            currentText = null;
            currentAttributes.Clear();
        }

        private void ReadPreviousElement()
        {
            var element = currentElement;

            if (element == null)
            {
                return;
            }

            currentElement = null;
            currentComponent = renderer.GetComponent(element);
            currentComponent.Render(this, this);
        }

        private void ReadAttribute(string name, string value)
        {
            currentAttributes[name] = value;
        }

        public string? GetAttribute(string name)
        {
            if (currentAttributes.TryGetValue(name, out var attribute))
            {
                return attribute;
            }

            if (currentComponent?.DefaultAttributes?.TryGetValue(name, out attribute) == true)
            {
                return attribute;
            }

            return null;
        }

        public IElementHtmlRenderer StartElement(string elementName)
        {
            currentRenderStyles.Clear();
            currentRenderAttributes.Clear();
            currentRenderElement = elementName;

            return this;
        }

        public void EndElement()
        {
            if (currentRenderElement == null)
            {
                return;
            }

            for (var i = 0; i < intend; i++)
            {
                sb.Append(' ');
            }

            sb.Append("</");
            sb.Append(currentRenderElement);
            sb.Append('>');
            sb.AppendLine();

            intend -= 2;
        }

        public void RenderChildren()
        {
            Read();
        }

        public IElementHtmlRenderer SetAttribute(string name, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return this;
            }

            currentRenderAttributes[name] = value;

            return this;
        }

        public IElementHtmlRenderer SetStyle(string name, string? value)
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

            intend += 2;

            for (var i = 0; i < intend; i++)
            {
                sb.Append(' ');
            }

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
            sb.AppendLine();

            return this;
        }

        public void SetContext(string name, object? value)
        {
            context[name] = value;
        }

        public object? GetContext(string name)
        {
            return context.GetValueOrDefault(name);
        }
    }
}
