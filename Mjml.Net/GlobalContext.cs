using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed class GlobalContext
    {
        private readonly RenderStack<TransitiveContext> transitive = new RenderStack<TransitiveContext>();
        private readonly Dictionary<string, Dictionary<string, string>> attributesByName = new Dictionary<string, Dictionary<string, string>>(10);
        private readonly Dictionary<string, Dictionary<string, string>> attributesByClass = new Dictionary<string, Dictionary<string, string>>(10);

        public Dictionary<(Type Type, string Name), object> GlobalData { get; } = new Dictionary<(Type Type, string Name), object>();

        public Dictionary<string, Dictionary<string, string>> AttributesByClass => attributesByClass;

        public Dictionary<string, Dictionary<string, string>> AttributesByName => attributesByName;

        public MjmlOptions Options { get; private set; }

        public GlobalContext()
        {
            transitive.Push(new TransitiveContext(null));
        }

        public void SetOptions(MjmlOptions options)
        {
            Options = options;
        }

        public void Clear()
        {
            GlobalData.Clear();

            attributesByClass.Clear();
            attributesByName.Clear();

            transitive.Clear();
            transitive.Push(new TransitiveContext(null));
        }

        public void SetGlobalData(string name, object? value)
        {
            if (value == null)
            {
                return;
            }

            GlobalData[(value.GetType(), name)] = value;
        }

        public void SetTypeAttribute(string name, string type, string value)
        {
            if (!attributesByName.TryGetValue(name, out var attributes))
            {
                attributes = new Dictionary<string, string>();

                attributesByName[name] = attributes;
            }

            attributes[type] = value;
        }

        public void SetClassAttribute(string name, string className, string value)
        {
            if (!attributesByClass.TryGetValue(className, out var attributes))
            {
                attributes = new Dictionary<string, string>();

                attributesByClass[className] = attributes;
            }

            attributes[name] = value;
        }

        public string? GetAttribute(string elementName, string[]? classes)
        {
            if (attributesByName.TryGetValue(elementName, out var byType))
            {
                if (byType.TryGetValue(elementName, out var attribute))
                {
                    return attribute;
                }

                if (byType.TryGetValue(Constants.All, out attribute))
                {
                    return attribute;
                }
            }

            if (attributesByClass.Count > 0 && classes != null)
            {
                foreach (var className in classes)
                {
                    if (attributesByClass.TryGetValue(className, out var byName))
                    {
                        if (byName.TryGetValue(elementName, out var attribute))
                        {
                            return attribute;
                        }
                    }
                }
            }

            return null;
        }

        public void Push()
        {
            transitive.Push(new TransitiveContext(transitive.Current));
        }

        public void Pop()
        {
            transitive.Pop();
        }

        public object? Set(string name, object? value)
        {
            return transitive.Current?.Set(name, value);
        }

        public object? Get(string name)
        {
            return transitive.Current?.Get(name);
        }
    }
}
