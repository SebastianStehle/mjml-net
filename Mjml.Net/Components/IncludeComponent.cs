﻿using Mjml.Net.Components.Body;
using Mjml.Net.Components.Head;
using Mjml.Net.Helpers;
using Mjml.Net.Types;

namespace Mjml.Net.Components;

public sealed partial class IncludeComponent : Component
{
    public override string ComponentName => "mj-include";

    public override bool Raw => true;

    [Bind("path", BindType.RequiredString)]
    public string Path;

    [Bind("css-inline", BindType.Inline)]
    public string? CssInline;

    [Bind("type", typeof(TypeValidator))]
    public string Type;

    public IncludeType ActualType { get; private set; }

    public override void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context)
    {
        var actualPath = Binder.GetAttribute("path");
        var actualType = Binder.GetAttribute("type");

        switch (actualType)
        {
            case "html":
                ActualType = IncludeType.Html;
                break;
            case "css":
                ActualType = IncludeType.Css;
                break;
            case "mjml":
                ActualType = IncludeType.Mjml;
                break;
            default:
                if (actualPath?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Html;
                }
                else if (actualPath?.EndsWith(".css", StringComparison.OrdinalIgnoreCase) == true)
                {
                    ActualType = IncludeType.Css;
                }
                else
                {
                    ActualType = IncludeType.Mjml;
                }

                break;
        }

        if (ActualType != IncludeType.Mjml || string.IsNullOrWhiteSpace(actualPath))
        {
            return;
        }

        var content = context.FileLoader?.LoadText(actualPath);

        if (!string.IsNullOrWhiteSpace(content))
        {
            // Add the new element to the include parent, so actually after the include to have a correct parent-child relationship.
            mjmlReader.ReadFragment(content, actualPath, this);

            static void AddToHead(IComponent component, IComponent parent)
            {
                // Go to the root element to find the parent.
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }

                if (parent is not RootComponent root)
                {
                    return;
                }

                var head = root.ChildNodes.OfType<HeadComponent>().FirstOrDefault();

                // If there is no head element in the current tree, add one.
                if (head == null)
                {
                    head = new HeadComponent();

                    root.InsertChild(head, 0);
                }

                foreach (var child in component.ChildNodes)
                {
                    Add(child, head);
                }
            }

            static void Add(IComponent component, IComponent parent)
            {
                if (component is HeadComponent)
                {
                    // Add head children to the root head.
                    AddToHead(component, parent);
                }
                else if (component is RootComponent or BodyComponent or IncludeComponent)
                {
                    // Just ignore these component and add the children to the parent.
                    foreach (var child in component.ChildNodes)
                    {
                        Add(child, parent);
                    }
                }
                else
                {
                    parent.AddChild(component);
                }
            }

            if (Parent != null)
            {
                Add(this, Parent);
            }

            // The children have been added to our parent or to the head element.
            ClearChildren();
        }
    }

    public override void Measure(GlobalContext context, double parentWidth, int numSiblings, int numNonRawSiblings)
    {
        base.Measure(context, parentWidth, numSiblings, numNonRawSiblings);
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        if (ActualType == IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
        {
            // Render the children that have been added in the bind method.
            RenderChildren(renderer, context);
            return;
        }

        // The file context is not needed here, because we have no inner rendering.
        var content = context.FileLoader?.LoadText(Path);

        if (content == null)
        {
            return;
        }

        if (ActualType == IncludeType.Html)
        {
            // Allow pretty formatting and indentation.
            renderer.Content(content);
        }
        else if (ActualType == IncludeType.Css)
        {
            var isInline = string.Equals(CssInline, "inline", StringComparison.OrdinalIgnoreCase);

            // Allow multiple styles and render them later.
            context.AddGlobalData(Style.Static(new InnerTextOrHtml(content), isInline));
        }
    }

    internal sealed class TypeValidator : EnumType
    {
        public TypeValidator()
            : base(false, "mjml", "html", "css")
        {
        }
    }
}
