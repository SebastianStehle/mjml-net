namespace Mjml.Net;

public record struct AttributeKey(string ClassOrType, string Name);

public record struct AttributeParentKey(string ParentClass, string ClassOrType, string Name);

public sealed class GlobalContext
{
    private readonly Dictionary<AttributeKey, string> attributesByName = new Dictionary<AttributeKey, string>(10);
    private readonly Dictionary<AttributeKey, string> attributesByClass = new Dictionary<AttributeKey, string>(10);
    private readonly Dictionary<AttributeParentKey, string> attributesByParentClass = new Dictionary<AttributeParentKey, string>(10);
    private readonly Dictionary<string, Dictionary<string, string>> attributesByType = [];
    private readonly Dictionary<string, Dictionary<string, string>> attributesByClassName = [];
    private readonly Dictionary<(string ParentClass, string Type), Dictionary<string, string>> attributesByParentClassAndType = [];
    private IFileLoader? fileLoader;
    private string? breakpoint;

    public Dictionary<(Type Type, object Identifier), GlobalData> GlobalData { get; } = [];

    public IReadOnlyDictionary<AttributeKey, string> AttributesByClass => attributesByClass;

    public IReadOnlyDictionary<AttributeParentKey, string> AttributesByParentClass => attributesByParentClass;

    public IReadOnlyDictionary<AttributeKey, string> AttributesByName => attributesByName;

    public MjmlOptions Options { get; set; }

    public bool Async { get; set; }

    public string Breakpoint
    {
        get => breakpoint ?? Options?.Breakpoint ?? "480px";
        set => breakpoint = value;
    }

    public IFileLoader? FileLoader
    {
        get => fileLoader ??= Options?.FileLoader?.Invoke();
    }

    public void Clear()
    {
        GlobalData.Clear();
        fileLoader = null;
        breakpoint = null;
        attributesByClass.Clear();
        attributesByName.Clear();
        attributesByParentClass.Clear();
        attributesByType.Clear();
        attributesByClassName.Clear();
        attributesByParentClassAndType.Clear();
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

    public void ClearGlobalData()
    {
        GlobalData.Clear();
    }

    public void SetTypeAttribute(string name, string type, string value)
    {
        attributesByName[new AttributeKey(type, name)] = value;

        // Also index the attributes by type, so that binding can iterate over the attributes of an element type.
        GetOrAddAttributes(attributesByType, type)[name] = value;
    }

    internal Dictionary<string, string>? GetTypeAttributes(string type)
    {
        return attributesByType.GetValueOrDefault(type);
    }

    public void SetClassAttribute(string name, string className, string value)
    {
        attributesByClass[new AttributeKey(className, name)] = value;

        // Also index the attributes by class, so that binding does not have to iterate over the attributes of all classes for each class of an element.
        GetOrAddAttributes(attributesByClassName, className)[name] = value;
    }

    internal Dictionary<string, string>? GetClassAttributes(string className)
    {
        return attributesByClassName.GetValueOrDefault(className);
    }

    public void SetParentClassAttribute(string name, string parentClassName, string type, string value)
    {
        attributesByParentClass[new AttributeParentKey(parentClassName, type, name)] = value;

        GetOrAddAttributes(attributesByParentClassAndType, (parentClassName, type))[name] = value;
    }

    internal Dictionary<string, string>? GetParentClassAttributes(string parentClassName, string type)
    {
        return attributesByParentClassAndType.GetValueOrDefault((parentClassName, type));
    }

    private static Dictionary<string, string> GetOrAddAttributes<TKey>(Dictionary<TKey, Dictionary<string, string>> source, TKey key) where TKey : notnull
    {
        if (!source.TryGetValue(key, out var attributes))
        {
            attributes = [];
            source[key] = attributes;
        }

        return attributes;
    }
}
