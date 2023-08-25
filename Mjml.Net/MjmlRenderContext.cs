﻿using HtmlPerformanceKit;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed partial class MjmlRenderContext : IMjmlReader
{
    private readonly GlobalContext context = new GlobalContext();
    private readonly ValidationErrors errors = new ValidationErrors();
    private readonly List<Binder> allBinders = new List<Binder>();
    private ValidationContext validationContext;
    private MjmlOptions mjmlOptions;
    private MjmlRenderer mjmlRenderer;

    public ValidationErrors Validate()
    {
        return new ValidationErrors(errors);
    }

    public void Setup(MjmlRenderer mjmlRenderer, MjmlOptions? mjmlOptions)
    {
        this.mjmlRenderer = mjmlRenderer;
        this.mjmlOptions = mjmlOptions ??= new MjmlOptions();

        // Reuse the context and therefore do not set them over the constructor.
        context.SetOptions(mjmlOptions);

        validationContext.Options = mjmlOptions;
    }

    internal void Clear()
    {
        allBinders.Clear();
        context.Clear();
        mjmlOptions = null!;
        mjmlRenderer = null!;
        errors.Clear();

        ClearRenderData();
    }

    public void ReadFragment(string mjml, string? file, IComponent parent)
    {
        var reader = new HtmlReaderWrapper(mjml);

        Read(reader, parent, file);
    }

    public void Read(IHtmlReader reader, IComponent? parent, string? file)
    {
        while (reader.Read())
        {
            switch (reader.TokenKind)
            {
                case HtmlTokenKind.Tag:
                    ReadElement(reader.Name, reader, parent, file);
                    break;
                case HtmlTokenKind.Comment when mjmlOptions.KeepComments && parent != null:
                    ReadComment(reader, parent);
                    break;
                case HtmlTokenKind.EndTag:
                    return;
            }
        }
    }

    private void ReadElement(string name, IHtmlReader reader, IComponent? parent, string? file)
    {
        var component = mjmlRenderer.CreateComponent(name);

        var position = new SourcePosition(
            reader.LineNumber,
            reader.LinePosition,
            file);

        if (component == null)
        {
            errors.Add($"Invalid element '{name}'.",
                ValidationErrorType.UnknownElement,
                position);
            return;
        }

        parent?.AddChild(component);

        var binder = DefaultPools.Binders.Get().Setup(context, parent, component.ComponentName);

        allBinders.Add(binder);

        for (var i = 0; i < reader.AttributeCount; i++)
        {
            var attributeName = reader.GetAttributeName(i);
            var attributeValue = reader.GetAttribute(i);

            binder.SetAttribute(attributeName, attributeValue);

            validationContext.Position = new SourcePosition(
                reader.LineNumber,
                reader.LinePosition,
                file);

            mjmlOptions.Validator?.Attribute(attributeName, attributeValue, component, errors, ref validationContext);
        }

        if (component.ContentType == ContentType.Text)
        {
            binder.SetText(reader.ReadInnerText());
        }
        else if (component.ContentType == ContentType.Raw)
        {
            component.AddChild(reader.ReadInnerHtml());
        }

        component.SetBinder(binder);
        component.Read(reader, this, context);
        component.Position = position;

        if (!reader.SelfClosingElement && reader.TokenKind != HtmlTokenKind.EndTag)
        {
            Read(reader, component, file);
        }

        if (parent == null)
        {
            component.Bind(context);
            component.Measure(context, 600, 0, 0);
            component.Render(this, context);

            mjmlOptions.Validator?.Components(component, errors, ref validationContext);

            Cleanup();
        }
    }

    private void Cleanup()
    {
        foreach (var usedBinder in allBinders)
        {
            DefaultPools.Binders.Return(usedBinder);
        }
    }

    private static void ReadComment(IHtmlReader reader, IComponent parent)
    {
        parent.AddChild(new CommentComponent
        {
            Text = reader.Text
        });
    }
}
