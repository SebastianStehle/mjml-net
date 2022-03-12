using System.Text;
using System.Xml;

namespace ConsoleApp22
{
    internal partial class MjmlRenderContext : INode
    {
        private readonly MjmlRenderer renderer;
        private readonly XmlReader reader;
        private readonly Dictionary<string, string> currentAttributes = new Dictionary<string, string>(10);
        private readonly Dictionary<string, object?> context = new Dictionary<string, object?>();
        private IComponent? currentComponent;
        private string? currentText;
        private string? currentElement;
        private int intend = -2;

        public MjmlRenderContext(MjmlRenderer renderer, XmlReader reader)
        {
            this.renderer = renderer;
            this.reader = reader;
        }

        public void Read()
        {
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ReadElement(reader.Name);
                        return;
                }
            }
            while (reader.Read());
        }

        private void ReadElement(string name)
        {
            var isStopped = false;

            currentText = null;
            currentElement = name;
            currentAttributes.Clear();

            while (!isStopped && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        currentAttributes[reader.Value] = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        currentText = reader.Value;
                        isStopped = true;
                        break;
                    case XmlNodeType.Element:
                    case XmlNodeType.EndElement:
                        isStopped = true;
                        break;
                }
            }

            var element = currentElement;

            if (element == null)
            {
                throw new InvalidOperationException("Invalid element {elementName}.");
            }

            currentComponent = renderer.GetComponent(element);
            currentComponent.Render(this, this);
        }

        public string? GetAttribute(string name, string? fallback = null)
        {
            if (currentAttributes.TryGetValue(name, out var attribute))
            {
                return attribute;
            }

            if (currentComponent?.DefaultAttributes?.TryGetValue(name, out attribute) == true)
            {
                return attribute;
            }

            return fallback;
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
