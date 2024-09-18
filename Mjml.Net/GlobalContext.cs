using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed class GlobalContext
{
    private readonly Dictionary<string, Dictionary<string, string>> attributesByName = new Dictionary<string, Dictionary<string, string>>(10);
    private readonly Dictionary<string, Dictionary<string, string>> attributesByClass = new Dictionary<string, Dictionary<string, string>>(10);
    private IFileLoader? fileLoader;

    public Dictionary<(Type Type, object Identifier), GlobalData> GlobalData { get; } = [];

    public Dictionary<string, Dictionary<string, string>> AttributesByClass => attributesByClass;

    public Dictionary<string, Dictionary<string, string>> AttributesByName => attributesByName;

    public MjmlOptions Options { get; set; }

    public bool Async { get; set; }

    public IFileLoader? FileLoader
    {
        get => fileLoader ??= Options?.FileLoader?.Invoke();
    }

    public void Clear()
    {
        GlobalData.Clear();
        fileLoader = null;
        attributesByClass.Clear();
        attributesByName.Clear();
        Options = null!;
    }

    public void SetGlobalData<T>(object identifier, T value, bool doNotOverride = false) where T : GlobalData
    {
        var key = (typeof(T), identifier);

        if (doNotOverride && GlobalData.ContainsKey(key))
        {
            return;
        }

        GlobalData[key] = value;
    }

    public void AddGlobalData<T>(T value) where T : GlobalData
    {
        var key = (typeof(T), Guid.NewGuid());

        GlobalData[key] = value;
    }

    public void SetTypeAttribute(string name, string type, string value)
    {
        if (!attributesByName.TryGetValue(name, out var attributes))
        {
            attributes = [];
            attributesByName[name] = attributes;
        }

        attributes[type] = value;
    }

    public void SetClassAttribute(string name, string className, string value)
    {
        if (!attributesByClass.TryGetValue(className, out var attributes))
        {
            attributes = [];
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
