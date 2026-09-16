using HtmlPerformanceKit;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed partial class MjmlRenderContext : IMjmlReader
{
    private readonly GlobalContext context = new GlobalContext();
    private readonly ValidationErrors errors = [];
    private ValidationContext validationContext;
    private MjmlOptions mjmlOptions;
    private MjmlRenderer mjmlRenderer;
    private bool hasAddedClosingError;
    private Action<HtmlError>? onError;
    private string? currentFile;

    public ValidationErrors Validate()
    {
        return new ValidationErrors(errors);
    }

    public void Setup(MjmlRenderer mjmlRenderer, bool isAsync, MjmlOptions? mjmlOptions)
    {
        this.mjmlRenderer = mjmlRenderer;
        this.mjmlOptions = mjmlOptions ??= new MjmlOptions();

        // Reuse the context and therefore do not set them over the constructor.
        context.Options = mjmlOptions;
        context.Async = isAsync;

        validationContext.Options = mjmlOptions;
    }

    internal void Clear()
    {
        context.Clear();
        mjmlOptions = null!;
        mjmlRenderer = null!;
        errors.Clear();
        hasAddedClosingError = false;
        currentFile = null;

        ClearRenderData();
    }

    public void ReadFragment(string mjml, string? file, IComponent parent)
    {
        var reader = new HtmlReaderWrapper(mjml);

        Read(reader, parent, file);
    }

    public void Read(IHtmlReader reader, IComponent? parent, string? file)
    {
        // Use a single handler instead of a closure per call. The file is restored afterwards, because includes are read with another file.
        var previousFile = currentFile;

        currentFile = file;
        reader.OnError = onError ??= OnError;

        try
        {
            var subTree = reader.ReadSubtree();

            // Only add the text error once per parent.
            var hasAddedError = false;

            while (subTree.Read())
            {
                switch (subTree.TokenKind)
                {
                    case HtmlTokenKind.Tag:
                        ReadElement(subTree, parent, file);
                        break;
                    case HtmlTokenKind.Comment when mjmlOptions.KeepComments && parent != null:
                        ReadComment(subTree, parent);
                        break;
                    case HtmlTokenKind.Text when !hasAddedError && subTree.TextAsSpan.Trim().Length > 0:
                        errors.Add(
                            "Unexpected text content.",
                            ValidationErrorType.UnexpectedText,
                            Position(reader, file));

                        // Only add the text error once per parent.
                        hasAddedError = true;
                        break;
                }
            }
        }
        finally
        {
            reader.OnError = null;

            currentFile = previousFile;
        }
    }

    private void OnError(HtmlError error)
    {
        errors.Add(
            new ValidationError(
                error.Message,
                ValidationErrorType.InvalidHtml,
                new SourcePosition(
                    error.LineNumber,
                    error.LinePosition,
                    currentFile)));
    }

    private void ReadElement(IHtmlReader reader, IComponent? parent, string? file)
    {
        var component = mjmlRenderer.CreateComponent(reader.NameAsSpan);

        var position = new SourcePosition(
            reader.LineNumber,
            reader.LinePosition,
            file);

        if (component == null)
        {
            errors.Add(
                $"Invalid element '{reader.Name}'.",
                ValidationErrorType.UnknownElement,
                Position(reader, file));

            // Just skip unknown elements.
            reader.ReadInnerHtml();
            return;
        }

        parent?.AddChild(component);

        var binder = DefaultPools.Binders.Get().Setup(context, parent, component.ComponentName);

        BindComponentAttributes(reader, component, binder, file);
        BindComponentContent(reader, component, binder);

        component.SetBinder(binder);
        component.Read(reader, this, context);
        component.Position = position;

        if (reader.TokenKind == HtmlTokenKind.Tag && !reader.SelfClosingElement)
        {
            Read(reader, component, file);
        }

        ValidatingClosingState(component.ComponentName, reader);

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
            var attributeName = GetAttributeName(reader, i, component);
            var attributeValue = reader.GetAttribute(i);

            binder.SetAttribute(attributeName, attributeValue);

            validationContext.Position = new SourcePosition(
                reader.LineNumber,
                reader.LinePosition,
                file);

            mjmlOptions.Validator?.Attribute(attributeName, attributeValue, component, errors, ref validationContext);
        }
    }

    private static string GetAttributeName(IHtmlReader reader, int index, IComponent component)
    {
#if NET9_0_OR_GREATER
        // Reuse the name from the allowed attributes of the component instead of allocating a new string for every element.
        var allowedFields = component.AllowedFields;

        if (allowedFields != null && allowedFields.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(reader.GetAttributeNameAsSpan(index), out var name, out _))
        {
            return name;
        }
#endif
        return reader.GetAttributeName(index);
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

        Cleanup(component);
    }

    private void ValidatingClosingState(string name, IHtmlReader reader)
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

        if (reader.TokenKind == HtmlTokenKind.EndTag && !reader.NameAsSpan.SequenceEqual(name))
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

    private static void Cleanup(IComponent component)
    {
        if (component.Binder is Binder binder)
        {
            DefaultPools.Binders.Return(binder);
            component.SetBinder(null!);
        }

        if (component is Component typed)
        {
            foreach (var child in typed.ChildNodes)
            {
                Cleanup(child);
            }
        }
        else
        {
            foreach (var child in component.ChildNodes)
            {
                Cleanup(child);
            }
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
