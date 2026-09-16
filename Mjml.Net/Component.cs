namespace Mjml.Net;

public abstract class Component : IComponent
{
    private static readonly List<IComponent> EmptyComponents = [];
    private List<IComponent>? childNodes;
    private List<InnerTextOrHtml>? childInput;
    private IBinder binder;

    // A list instead of IEnumerable, so that foreach does not box the enumerator. Use AddChild or InsertChild to add children.
    public List<IComponent> ChildNodes
    {
        get => childNodes ?? EmptyComponents;
    }

    IEnumerable<IComponent> IComponent.ChildNodes => ChildNodes;

    public double ActualWidth { get; protected set; }

    public IComponent? Parent { get; set; }

    public IBinder Binder => binder;

    public SourcePosition Position { get; set; }

    public virtual bool Raw => false;

    public virtual AllowedParents? AllowedParents => null;

    public virtual AllowedAttributes? AllowedFields => null;

    public virtual ContentType ContentType => ContentType.Complex;

    public abstract string ComponentName { get; }

    public abstract void Render(IHtmlRenderer renderer, GlobalContext context);

    public virtual void RenderChildren(IHtmlRenderer renderer, GlobalContext context)
    {
        if (childNodes == null)
        {
            return;
        }

        foreach (var child in childNodes)
        {
            child.Render(renderer, context);
        }
    }

    protected virtual void RenderRaw(IHtmlRenderer renderer)
    {
        if (childInput == null)
        {
            return;
        }

        foreach (var child in childInput)
        {
            renderer.Plain(child);
        }
    }

    public virtual string? GetInheritingAttribute(string name)
    {
        return null;
    }

    public virtual string? GetDefaultValue(string name)
    {
        return null;
    }

    public virtual string? GetAttribute(string name)
    {
        return null;
    }

    public void AddChild(InnerTextOrHtml rawInput)
    {
        childInput ??= new List<InnerTextOrHtml>(1);
        childInput.Add(rawInput);
    }

    public void AddChild(IComponent child)
    {
        child.Parent = this;

        childNodes ??= new List<IComponent>(1);
        childNodes.Add(child);
    }

    public void InsertChild(IComponent child, int index)
    {
        child.Parent = this;

        childNodes ??= new List<IComponent>(1);
        childNodes.Insert(index, child);
    }

    public virtual void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context)
    {
    }

    protected virtual void ClearChildren()
    {
        childNodes?.Clear();
    }

    protected virtual void BeforeBind(GlobalContext context)
    {
    }

    protected virtual void AfterBind(GlobalContext context)
    {
    }

    public virtual void Measure(GlobalContext context, double parentWidth, int numSiblings, int numNonRawSiblings)
    {
        ActualWidth = parentWidth;

        MeasureChildren(context, ActualWidth);
    }

    public void SetBinder(IBinder binder)
    {
        this.binder = binder;
    }

    protected void MeasureChildren(GlobalContext context, double width)
    {
        if (childNodes != null)
        {
            var numNonRawSiblings = childNodes.Count(x => !x.Raw);

            foreach (var child in childNodes)
            {
                child.Measure(context, width, childNodes.Count, numNonRawSiblings);
            }
        }
    }

    public virtual void Bind(GlobalContext context)
    {
        BeforeBind(context);

        if (childNodes != null)
        {
            foreach (var child in childNodes)
            {
                child.Bind(context);
            }
        }

        AfterBind(context);
    }
}
