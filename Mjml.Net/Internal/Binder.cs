namespace Mjml.Net.Internal
{
    internal sealed class Binder : IBinder
    {
        private static readonly char[] TrimChars = { ' ', '\n', '\r' };
        private readonly GlobalContext context;
        private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();
        private IComponent? parent;
        private string elementName;
        private string? currentText;
        private string[]? currentClasses;

        public Binder(GlobalContext context)
        {
            this.context = context;
        }

        public Binder Clear(IComponent? newParent, string? newElementName = null)
        {
            attributes.Clear();
            currentClasses = null;
            currentText = null;
            elementName = newElementName!;
            parent = newParent;

            return this;
        }

        public void SetAttribute(string name, string value)
        {
            attributes[name] = value;
        }

        public void SetText(string text)
        {
            currentText = text.Trim(TrimChars);
        }

        public string? GetAttribute(string name)
        {
            if (attributes.TryGetValue(name, out var attribute))
            {
                return attribute;
            }

            var inherited = parent?.GetInheritingAttribute(name);

            if (inherited != null)
            {
                return inherited;
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

                string? classAttribute = null;

                // Loop over all classes and use the last match.
                foreach (var className in currentClasses)
                {
                    if (context.AttributesByClass.TryGetValue(className, out var byName))
                    {
                        if (byName.TryGetValue(name, out attribute))
                        {
                            classAttribute = attribute;
                        }
                    }
                }

                if (classAttribute != null)
                {
                    return classAttribute;
                }
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

            return null;
        }

        public string? GetText()
        {
            return currentText;
        }
    }
}
