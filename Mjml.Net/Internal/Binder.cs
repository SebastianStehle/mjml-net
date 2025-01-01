using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

namespace Mjml.Net.Internal;

internal sealed class Binder : IBinder
{
    private readonly Dictionary<string, string> attributes = [];
    private GlobalContext context;
    private IComponent? elementParent;
    private InnerTextOrHtml? currentText;
    private string elementName;
    private string[]? currentClasses;

    public string[] ClassNames
    {
        get
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

            return currentClasses;
        }
    }

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
        if (attributes.TryGetValue(name, out var a1))
        {
            return a1;
        }

        var inherited = elementParent?.GetInheritingAttribute(name);
        if (inherited != null)
        {
            return inherited;
        }

        if (context.AttributesByClass.Count > 0)
        {
            var classNames = ClassNames;
            if (classNames.Length > 0)
            {
                string? classAttribute = null;
                // Loop over all classes and use the last match.
                foreach (var className in classNames)
                {
                    if (context.AttributesByClass.TryGetValue(new AttributeKey(className, name), out var a2))
                    {
                        classAttribute = a2;
                    }
                }

                if (classAttribute != null)
                {
                    return classAttribute;
                }
            }
        }

        if (context.AttributesByParentClass.Count > 0 && elementParent != null)
        {
            var classNames = elementParent.Binder.ClassNames;
            if (classNames.Length > 0)
            {
                string? classAttribute = null;
                // Loop over all classes and use the last match.
                foreach (var className in classNames)
                {
                    if (context.AttributesByParentClass.TryGetValue(new AttributeParentKey(className, elementName, name), out var a3))
                    {
                        classAttribute = a3;
                    }
                }

                if (classAttribute != null)
                {
                    return classAttribute;
                }
            }
        }

        if (context.AttributesByName.TryGetValue(new AttributeKey(elementName, name), out var a4))
        {
            return a4;
        }

        if (context.AttributesByName.TryGetValue(new AttributeKey(Constants.All, name), out var a5))
        {
            return a5;
        }

        return null;
    }

    private static string? GetByClass(IReadOnlyDictionary<AttributeKey, string> attributes, string[] classNames, string name)
    {
        string? result = null;
        // Loop over all classes and use the last match.
        foreach (var className in classNames)
        {
            if (attributes.TryGetValue(new AttributeKey(className, name), out var a))
            {
                result = a;
            }
        }

        return result;
    }

    public InnerTextOrHtml? GetText()
    {
        return currentText;
    }
}
