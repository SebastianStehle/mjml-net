using HtmlPerformanceKit;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed partial class MjmlRenderContext : IMjmlReader
{
    private readonly GlobalContext context = new GlobalContext();
    private readonly ValidationErrors errors = [];
    private readonly List<Binder> allBinders = [];
    private ValidationContext validationContext;
    private MjmlOptions mjmlOptions;
    private MjmlRenderer mjmlRenderer;
    private bool hasAddedClosingError;

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
        hasAddedClosingError = false;

        ClearRenderData();
    }

    public void ReadFragment(string mjml, string? file, IComponent parent)
    {
        var reader = new HtmlReaderWrapper(mjml);

        Read(reader, parent, file);
    }

    public void Read(IHtmlReader reader, IComponent? parent, string? file)
    {
        reader.OnError = error => errors.Add(
            new ValidationError(
                error.Message,
                ValidationErrorType.InvalidHtml,
                new SourcePosition(
                    error.LineNumber,
                    error.LinePosition,
                    file)));

        try
        {
            var substree = reader.ReadSubtree();

            // Only add the text error once per parent.
            var hasAddedError = false;

            while (substree.Read())
            {
                switch (substree.TokenKind)
                {
                    case HtmlTokenKind.Tag:
                        ReadElement(substree.Name, substree, parent, file);
                        break;
                    case HtmlTokenKind.Comment when mjmlOptions.KeepComments && parent != null:
                        ReadComment(substree, parent);
                        break;
                    case HtmlTokenKind.Text when !hasAddedError && substree.TextAsSpan.Trim().Length > 0:
                        errors.Add(
                            "Unexpected text content.",
                            ValidationErrorType.UnexpectedText,
                            Position(reader, file));

                        // Only add the text error once per parent.
                        hasAddedClosingError = true;
                        break;
                }
            }
        }
        finally
        {
            reader.OnError = null;
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
            errors.Add(
                $"Invalid element '{name}'.",
                ValidationErrorType.UnknownElement,
                Position(reader, file));

            // Just skip unknown elements.
            reader.ReadInnerHtml();
            return;
        }

        parent?.AddChild(component);

        var binder = DefaultPools.Binders.Get().Setup(context, parent, component.ComponentName);

        // Add all binders to list, so that we can return them later to the pool.
        allBinders.Add(binder);

        BindComponentAttributes(reader, component, binder, file);
        BindComponentContent(reader, component, binder);

        component.SetBinder(binder);
        component.Read(reader, this, context);
        component.Position = position;

        if (reader.TokenKind == HtmlTokenKind.Tag && !reader.SelfClosingElement)
        {
            Read(reader, component, file);
        }

        ValidatingClosingState(name, reader, file);

        // If there is no parent, we handle the root and we can render everything top to bottom.
        if (parent == null)
        {
            BindAndRender(component);
        }
    }

    private void BindComponentAttributes(IHtmlReader reader, IComponent component, Binder binder, string? file)
    {
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
    }

    private static void BindComponentContent(IHtmlReader reader, IComponent component, Binder binder)
    {
        if (component.ContentType == ContentType.Text)
        {
            binder.SetText(reader.ReadInnerText());
        }
        else if (component.ContentType == ContentType.Raw)
        {
            component.AddChild(reader.ReadInnerHtml());
        }
    }

    private void BindAndRender(IComponent component)
    {
        component.Bind(context);
        component.Measure(context, 600, 0, 0);
        component.Render(this, context);

        mjmlOptions.Validator?.Components(component, errors, ref validationContext);

        Cleanup();
    }

    private void ValidatingClosingState(string name, IHtmlReader reader, string? file)
    {
        // Only show one closing error, otherwise we could get one for every item in the hierarchy.
        if (hasAddedClosingError)
        {
            return;
        }

        if (reader.TokenKind == HtmlTokenKind.Tag && reader.SelfClosingElement)
        {
            return;
        }

        if (reader.TokenKind == HtmlTokenKind.EndTag && reader.Name != name)
        {
            errors.Add(
                $"Unexpected end element, expected '{name}', got '{reader.Name}'.",
                ValidationErrorType.InvalidHtml,
                Position(reader, name));

            // Only show one closing error, otherwise we could get one for every item in the hierarchy.
            hasAddedClosingError = true;
        }
        else if (reader.TokenKind != HtmlTokenKind.EndTag)
        {
            errors.Add(
                $"Unexpected end element, expected '{name}', got '{reader.TokenKind}' token.",
                ValidationErrorType.InvalidHtml,
                Position(reader, name));

            // Only show one closing error, otherwise we could get one for every item in the hierarchy.
            hasAddedClosingError = true;
        }
    }

    private static SourcePosition Position(IHtmlReader reader, string? file)
    {
        return new SourcePosition(reader.LineNumber, reader.LinePosition, file);
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
