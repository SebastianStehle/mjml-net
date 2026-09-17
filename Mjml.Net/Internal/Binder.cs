namespace Mjml.Net.Internal;

internal sealed class Binder : IBinder
{
    private readonly Dictionary<string, string> attributes = [];
    private GlobalContext context;
    private IComponent? elementParent;
    private InnerTextOrHtml? currentText;
    private string elementName;
    private string[]? currentClasses;
    private bool isResolved;
    private Action<string, string?>? addInheritingAttribute;

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

    public IReadOnlyDictionary<string, string> Attributes
    {
        get
        {
            if (!isResolved)
            {
                Resolve();
            }

            return attributes;
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
        isResolved = false;
    }

    public void SetAttribute(string name, string value)
    {
        attributes[name] = value;
    }

    public void SetText(InnerTextOrHtml text)
    {
        currentText = text;
    }

    public InnerTextOrHtml? GetText()
    {
        return currentText;
    }

    private void Resolve()
    {
        isResolved = true;

        // The classes must be read from the own attributes, before the other sources are added.
        var classNames = ClassNames;

        // The own attributes are already in the dictionary. Add the other sources from the highest to the lowest precedence.
        // Binders are pooled, so the callback is only created once per binder.
        elementParent?.AddInheritingAttributes(addInheritingAttribute ??= AddInheritingAttribute);

        if (context.AttributesByClass.Count > 0)
        {
            // The last class wins.
            for (var i = classNames.Length - 1; i >= 0; i--)
            {
                AddAll(context.GetClassAttributes(classNames[i]));
            }
        }

        if (context.AttributesByParentClass.Count > 0 && elementParent != null)
        {
            var parentClassNames = elementParent.Binder.ClassNames;

            // The last class wins.
            for (var i = parentClassNames.Length - 1; i >= 0; i--)
            {
                AddAll(context.GetParentClassAttributes(parentClassNames[i], elementName));
            }
        }

        AddAll(context.GetTypeAttributes(elementName));
        AddAll(context.GetTypeAttributes(Constants.All));
    }

    private void AddInheritingAttribute(string name, string? value)
    {
        if (value != null)
        {
            attributes.TryAdd(name, value);
        }
    }

    private void AddAll(Dictionary<string, string>? source)
    {
        if (source == null)
        {
            return;
        }

        foreach (var (name, value) in source)
        {
            attributes.TryAdd(name, value);
        }
    }
}
