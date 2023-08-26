namespace Mjml.Net.Internal;

internal sealed class Binder : IBinder
{
    private readonly Dictionary<string, string> attributes = new Dictionary<string, string>();
    private GlobalContext context;
    private IComponent? elementParent;
    private InnerTextOrHtml? currentText;
    private string elementName;
    private string[]? currentClasses;

    public Binder Setup(GlobalContext newContext, IComponent? newParent, string? newElementName = null)
    {
        context = newContext;
        elementName = newElementName!;
        elementParent = newParent;

        return this;
    }

    public void Clear()
    {
        attributes.Clear();
        context = null!;
        currentClasses = null;
        currentText = null;
        elementName = null!;
        elementParent = null!;
    }

    public void SetAttribute(string name, string value)
    {
        attributes[name] = value;
    }

    public void SetText(InnerTextOrHtml text)
    {
        currentText = text;
    }

    public string? GetAttribute(string name)
    {
        if (attributes.TryGetValue(name, out var attribute))
        {
            return attribute;
        }

        var inherited = elementParent?.GetInheritingAttribute(name);

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

    public InnerTextOrHtml? GetText()
    {
        return currentText;
    }
}
