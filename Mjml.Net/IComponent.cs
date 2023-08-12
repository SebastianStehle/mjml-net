﻿namespace Mjml.Net;

public interface IComponent
{
    AllowedParents? AllowedParents { get; }

    AllowedAttributes? AllowedFields { get; }

    IEnumerable<IComponent> ChildNodes { get; }

    ContentType ContentType { get; }

    bool Raw { get; }

    string ComponentName { get; }

    string? GetDefaultValue(string name);

    string? GetInheritingAttribute(string name);

    string? GetAttribute(string name);

    double ActualWidth { get; }

    void Bind(IBinder node, GlobalContext context, IHtmlReader reader);

    void AddChild(IComponent child);

    void AddChild(InnerTextOrHtml rawXml);

    void AfterBind(GlobalContext context, IHtmlReader reader, IMjmlReader mjmlReader);

    void Render(IHtmlRenderer renderer, GlobalContext context);

    void Measure(double parentWidth, int numSiblings, int numNonRawSiblings);
}
