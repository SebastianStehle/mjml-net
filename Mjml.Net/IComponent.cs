namespace Mjml.Net;

public interface IComponent
{
    AllowedParents? AllowedParents { get; }

    AllowedAttributes? AllowedFields { get; }

    IEnumerable<IComponent> ChildNodes { get; }

    IComponent? Parent { get; set; }

    ContentType ContentType { get; }

    bool Raw { get; }

    string ComponentName { get; }

    string? GetDefaultValue(string name);

    string? GetInheritingAttribute(string name);

    string? GetAttribute(string name);

    double ActualWidth { get; }

    void SetBinder(IBinder binder);

    void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context);

    void Bind(GlobalContext context);

    void AddChild(IComponent child);

    void AddChild(InnerTextOrHtml rawXml);

    void Render(IHtmlRenderer renderer, GlobalContext context);

    void Measure(GlobalContext context, double parentWidth, int numSiblings, int numNonRawSiblings);
}
