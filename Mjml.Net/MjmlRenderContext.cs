using System.Xml;
using HtmlPerformanceKit;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : IMjmlReader
    {
        private readonly GlobalContext context = new GlobalContext();
        private readonly ValidationErrors errors = new ValidationErrors();
        private readonly Binder binder;
        private ValidationContext validationContext;
        private MjmlOptions mjmlOptions;
        private MjmlRenderer mjmlRenderer;
        private IValidator? validator;
        private IComponent? currentComponent;

        public MjmlRenderContext()
        {
            binder = new Binder(context);
        }

        public MjmlRenderContext(MjmlRenderer renderer, MjmlOptions? mjmlOptions = null)
            : this()
        {
            Setup(renderer,  mjmlOptions);
        }

        public void Setup(MjmlRenderer mjmlRenderer, MjmlOptions? mjmlOptions)
        {
            this.mjmlRenderer = mjmlRenderer;
            this.mjmlOptions = mjmlOptions ??= new MjmlOptions();

            // We have to create a new instance each time, because it could another factory.
            validator = mjmlOptions.ValidatorFactory?.Create();

            // Reuse the context and therefore do not set them over the constructor.
            context.SetOptions(mjmlOptions);

            validationContext.Options = mjmlOptions;
        }

        internal void Clear()
        {
            context.Clear();
            mjmlOptions = null!;
            mjmlRenderer = null!;
            errors.Clear();

            ClearRenderData();
        }

        public ValidationErrors Validate()
        {
            return validator?.Complete() ?? new ValidationErrors();
        }

        public void ReadFragment(string mjml)
        {
            var fragmentReader = new HtmlReaderWrapper(mjml);

            using (fragmentReader)
            {
                Read(fragmentReader, currentComponent);
            }
        }

        public void Read(IHtmlReader reader, IComponent? parent)
        {
            while (reader.Read())
            {
                switch (reader.TokenKind)
                {
                    case HtmlTokenKind.Tag:
                        ReadElement(reader.Name, reader, parent);
                        break;
                    case HtmlTokenKind.Comment when mjmlOptions.KeepComments && parent != null:
                        ReadComment(reader, parent);
                        break;
                }
            }
        }

        private void ReadElement(string name, IHtmlReader parentReader, IComponent? parent)
        {
            var component = mjmlRenderer.CreateComponent(name);

            var currentLine = parentReader.LineNumber;
            var currentColumn = parentReader.LinePosition;

            if (component == null)
            {
                errors.Add($"Invalid element '{name}'.",
                    ValidationErrorType.UnknownElement,
                    currentLine,
                    currentColumn);
                return;
            }

            using (var reader = parentReader.ReadSubtree())
            {
                if (parent != null)
                {
                    parent.AddChild(component);
                }

                binder.Clear(parent, component.ComponentName);

                validationContext.LineNumber = currentLine;
                validationContext.LinePosition = currentColumn;

                validator?.BeforeComponent(component, ref validationContext);

                for (var i = 0; i < reader.AttributeCount; i++)
                {
                    var attributeName = reader.GetAttributeName(i);
                    var attributeValue = reader.GetAttribute(i);

                    binder.SetAttribute(attributeName, attributeValue);

                    validationContext.LineNumber = reader.LineNumber;
                    validationContext.LinePosition = reader.LinePosition;

                    validator?.Attribute(attributeName, attributeValue, component, ref validationContext);
                }

                if (component.ContentType == ContentType.Text)
                {
                    while (reader.Read())
                    {
                        switch (reader.TokenKind)
                        {
                            case HtmlTokenKind.Text:
                                binder.SetText(reader.Text);
                                break;
                        }
                    }
                }

                component.Bind(binder, context, reader);

                if (component.ContentType == ContentType.Raw)
                {
                    component.AddChild(reader.ReadInnerHtml());
                }
                else if (!reader.SelfClosingElement && reader.TokenKind != HtmlTokenKind.EndTag)
                {
                    Read(reader, component);
                }

                // Assign the current component, in case we read fragments.
                currentComponent = component;

                component.AfterBind(context, reader, this);
            }

            if (parent == null)
            {
                component.Measure(600, 0, 0);
                component.Render(this, context);
            }

            validationContext.LineNumber = currentLine;
            validationContext.LinePosition = currentColumn;

            validator?.AfterComponent(component, ref validationContext);
        }

        private static void ReadComment(IHtmlReader reader, IComponent parent)
        {
            parent.AddChild(new CommentComponent
            {
                Text = reader.Text
            });
        }
    }
}
