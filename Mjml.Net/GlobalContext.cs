namespace Mjml.Net;

public record struct AttributeKey(string ClassOrType, string Name);

public record struct AttributeParentKey(string ParentClass, string ClassOrType, string Name);

public sealed class GlobalContext
{
    private readonly Dictionary<AttributeKey, string> attributesByName = new Dictionary<AttributeKey, string>(10);
    private readonly Dictionary<AttributeKey, string> attributesByClass = new Dictionary<AttributeKey, string>(10);
    private readonly Dictionary<AttributeParentKey, string> attributesByParentClass = new Dictionary<AttributeParentKey, string>(10);
    private IFileLoader? fileLoader;

    public Dictionary<(Type Type, object Identifier), GlobalData> GlobalData { get; } = [];

    public IReadOnlyDictionary<AttributeKey, string> AttributesByClass => attributesByClass;

    public IReadOnlyDictionary<AttributeParentKey, string> AttributesByParentClass => attributesByParentClass;

    public IReadOnlyDictionary<AttributeKey, string> AttributesByName => attributesByName;

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

    public void ClearGlobalData()
    {
        GlobalData.Clear();
    }

    public void SetTypeAttribute(string name, string type, string value)
    {
        attributesByName[new AttributeKey(type, name)] = value;
    }

    public void SetClassAttribute(string name, string className, string value)
    {
        attributesByClass[new AttributeKey(className, name)] = value;
    }

    public void SetParentClassAttribute(string name, string parentClassName, string type, string value)
    {
        attributesByParentClass[new AttributeParentKey(parentClassName, type, name)] = value;
    }
}
