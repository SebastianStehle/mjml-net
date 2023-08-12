using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed class GlobalContext
{
    private readonly Dictionary<string, Dictionary<string, string>> attributesByName = new Dictionary<string, Dictionary<string, string>>(10);
    private readonly Dictionary<string, Dictionary<string, string>> attributesByClass = new Dictionary<string, Dictionary<string, string>>(10);

    public Dictionary<(Type Type, object Identifier), object> GlobalData { get; } = new Dictionary<(Type Type, object Identifier), object>();

    public Dictionary<string, Dictionary<string, string>> AttributesByClass => attributesByClass;

    public Dictionary<string, Dictionary<string, string>> AttributesByName => attributesByName;

    public MjmlOptions Options { get; private set; }

    public void SetOptions(MjmlOptions options)
    {
        Options = options;
    }

    public void Clear()
    {
        GlobalData.Clear();

        attributesByClass.Clear();
        attributesByName.Clear();

        Options = null!;
    }

    public void SetGlobalData<T>(object identifier, T? value, bool doNotOverride = false) where T : class
    {
        if (value == null)
        {
            return;
        }

        var key = (typeof(T), identifier);

        if (doNotOverride && GlobalData.ContainsKey(key))
        {
            return;
        }

        GlobalData[key] = value;
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
}
