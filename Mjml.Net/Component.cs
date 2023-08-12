﻿namespace Mjml.Net;

public abstract class Component : IComponent
{
    private List<IComponent>? childNodes;
    private List<InnerTextOrHtml>? childInput;

    public IEnumerable<IComponent> ChildNodes
    {
        get => childNodes ?? Enumerable.Empty<IComponent>();
    }

    public double ActualWidth { get; protected set; }

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
        childNodes ??= new List<IComponent>(1);
        childNodes.Add(child);
    }

    public void InsertChild(IComponent child, int index)
    {
        childNodes ??= new List<IComponent>(1);
        childNodes.Insert(index, child);
    }

    public virtual void Bind(IBinder binder, GlobalContext context, IHtmlReader reader)
    {
    }

    public virtual void AfterBind(GlobalContext context, IHtmlReader reader, IMjmlReader mjmlReader)
    {
    }

    public virtual void Measure(double parentWidth, int numSiblings, int numNonRawSiblings)
    {
        ActualWidth = parentWidth;

        MeasureChildren(ActualWidth);
    }

    protected void MeasureChildren(double width)
    {
        if (childNodes != null)
        {
            foreach (var child in childNodes)
            {
                child.Measure(width, childNodes.Count, childNodes.Count(x => !x.Raw));
            }
        }
    }
}
