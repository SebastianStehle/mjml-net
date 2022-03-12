using System.Xml;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : INode
    {
        private readonly MjmlRenderer renderer;
        private readonly XmlReader reader;
        private readonly MjmlOptions options;
        private readonly GlobalData globalData = new GlobalData();
        private readonly Dictionary<string, Dictionary<string, string>> defaultAttributes = new Dictionary<string, Dictionary<string, string>>();
        private readonly Dictionary<string, string> currentAttributes = new Dictionary<string, string>(10);
        private readonly Dictionary<string, object?> context = new Dictionary<string, object?>();
        private IComponent? currentComponent;
        private string? currentText;
        private string? currentName;
        private string? currentElement;

        public MjmlRenderContext(MjmlRenderer renderer, XmlReader reader, MjmlOptions options = default)
        {
            this.renderer = renderer;
            this.reader = reader;
            this.options = options;
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
            currentElement = name;
            currentName = currentElement.Substring(3); // Remove mj- prefix

            currentText = null;
            currentAttributes.Clear();

            var isStopped = false;

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

            currentComponent = renderer.GetComponent(currentElement);

            if (currentComponent == null)
            {
                throw new InvalidOperationException($"Invalid element '{currentElement}'.");
            }

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

            if (defaultAttributes.TryGetValue(name, out var attributesByName))
            {
                if (attributesByName.TryGetValue(currentName!, out attribute))
                {
                    return attribute;
                }

                if (attributesByName.TryGetValue(string.Empty, out attribute))
                {
                    return attribute;
                }
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

        public string? GetContent()
        {
            return currentText;
        }

        public void SetGlobalData(string name, object value, bool skipIfAdded = true)
        {
            var type = value.GetType().Name;

            if (skipIfAdded && globalData.ContainsKey((type, name)))
            {
                return;
            }

            globalData[(type, name)] = value;
        }

        public void SetDefaultAttribute(string name, string? type, string value)
        {
            if (!defaultAttributes.TryGetValue(name, out var attributes))
            {
                attributes = new Dictionary<string, string>();

                defaultAttributes[name] = attributes;
            }

            attributes[type ?? string.Empty] = value;
        }
    }
}
