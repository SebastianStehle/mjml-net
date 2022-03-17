namespace Mjml.Net.Internal
{
    internal sealed class Binder : INode
    {
        private static readonly char[] TrimChars = { ' ', '\n', '\r' };
        private readonly GlobalContext context;
        private readonly IComponent component;
        private readonly IComponent? parent;
        private readonly string elementName;
        private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();
        private string? currentText;
        private string[]? currentClasses;

        public Binder(GlobalContext context, IComponent component, IComponent? parent, string elementName)
        {
            this.context = context;
            this.component = component;
            this.parent = parent;
            this.elementName = elementName;
        }

        public void SetAttribute(string name, string value)
        {
            attributes[name] = value;
        }

        public void SetText(string text)
        {
            currentText = text;
        }

        public string? GetAttribute(string name, bool withoutDefaults = false)
        {
            if (attributes.TryGetValue(name, out var attribute))
            {
                return attribute;
            }

            if (context.AttributesByName.TryGetValue(name, out var byType))
            {
                if (byType.TryGetValue(elementName, out attribute))
                {
                    return attribute;
                }

                if (byType.TryGetValue(Constants.All, out attribute))
                {
                    return attribute;
                }
            }

            if (context.AttributesByClass.Count > 0)
            {
                if (currentClasses == null)
                {
                    if (attributes.TryGetValue(Constants.MjClass, out var classNames))
                    {
                        currentClasses = classNames.Split(' ');
                    }
                    else
                    {
                        currentClasses = Array.Empty<string>();
                    }
                }

                foreach (var className in currentClasses)
                {
                    if (context.AttributesByClass.TryGetValue(className, out var byName))
                    {
                        if (byName.TryGetValue(name, out attribute))
                        {
                            return attribute;
                        }
                    }
                }
            }

            if (parent != null)
            {
                var inherited = parent.GetInheritingAttribute(name);

                if (inherited != null)
                {
                    return inherited;
                }
            }

            if (!withoutDefaults)
            {
                return component?.Props?.DefaultValue(name);
            }

            return null;
        }

        public string? GetText()
        {
            return currentText;
        }
    }
}
