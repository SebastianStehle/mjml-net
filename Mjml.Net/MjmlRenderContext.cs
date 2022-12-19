using System.Xml;
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
            if (mjmlOptions.Lax)
            {
                mjml = XmlFixer.FixXML(mjml, mjmlOptions);
            }

            var fragmentReader = new XmlTextReader(mjml, XmlNodeType.Element, mjmlOptions.ParserContext)
            {
                // Parse the doctype definition for the allowed entities.
                DtdProcessing = DtdProcessing.Parse,

                // Keep the entities.
                EntityHandling = EntityHandling.ExpandCharEntities
            };

            using (fragmentReader)
            {
                ReadXml(fragmentReader, currentComponent);
            }
        }

        public void ReadXml(XmlReader reader, IComponent? parent)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ReadElement(reader.Name, reader, parent);
                        break;
                    case XmlNodeType.Comment when mjmlOptions.KeepComments && parent != null:
                        ReadComment(reader, parent);
                        break;
                }
            }
        }

        private void ReadElement(string name, XmlReader parentReader, IComponent? parent)
        {
            var component = mjmlRenderer.CreateComponent(name);

            var currentLine = CurrentLine(parentReader);
            var currentColumn = CurrentColumn(parentReader);

            if (component == null)
            {
                errors.Add($"Invalid element '{name}'.",
                    ValidationErrorType.UnknownElement,
                    currentLine,
                    currentColumn);
                return;
            }

            var reader = parentReader.ReadSubtree();

            if (parent != null)
            {
                parent.AddChild(component);
            }

            binder.Clear(parent, component.ComponentName);

            validationContext.XmlLine = currentLine;
            validationContext.XmlColumn = currentColumn;

            validator?.BeforeComponent(component, ref validationContext);

            reader.Read();

            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                var attributeName = reader.Name;
                var attributeValue = reader.Value;

                binder.SetAttribute(attributeName, attributeValue);

                validationContext.XmlLine = CurrentLine(reader);
                validationContext.XmlColumn = CurrentColumn(reader);

                validator?.Attribute(attributeName, attributeValue, component, ref validationContext);
            }

            if (component.ContentType == ContentType.Text)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Text:
                            binder.SetText(reader.Value);
                            break;
                    }
                }
            }

            component.Bind(binder, context, reader);

            if (component.ContentType == ContentType.Raw)
            {
                void Read()
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Comment:
                            component.AddChild($"<!-- {reader.Value} -->");

                            if (reader.NodeType != XmlNodeType.Comment)
                            {
                                Read();
                            }
                            break;

                        case XmlNodeType.Text:
                            component.AddChild(reader.Value);

                            if (reader.NodeType != XmlNodeType.Text && reader.NodeType != XmlNodeType.EntityReference)
                            {
                                Read();
                            }
                            break;
                        case XmlNodeType.Element:
                            component.AddChild(reader.ReadOuterXml().Trim());
                            Read();
                            break;

                        case XmlNodeType.EntityReference:
                            component.AddChild($"&{reader.Name};");
                            break;

                        default:
                            return;
                    }
                }

                while (reader.Read())
                {
                    Read();
                }
            }
            else
            {
                ReadXml(reader, component);
            }

            // Assign the current component, in case we read fragments.
            currentComponent = component;

            component.AfterBind(context, reader, this);

            reader.Close();

            if (parent == null)
            {
                component.Measure(600, 0, 0);
                component.Render(this, context);
            }

            validationContext.XmlLine = currentLine;
            validationContext.XmlColumn = currentColumn;

            validator?.AfterComponent(component, ref validationContext);
        }

        private static void ReadComment(XmlReader reader, IComponent parent)
        {
            parent.AddChild(new CommentComponent
            {
                Text = reader.Value
            });
        }

        private static int? CurrentLine(XmlReader reader)
        {
            if (reader is IXmlLineInfo lineInfo)
            {
                return lineInfo.LineNumber;
            }

            return null;
        }

        private static int? CurrentColumn(XmlReader reader)
        {
            if (reader is IXmlLineInfo lineInfo)
            {
                return lineInfo.LineNumber;
            }

            return null;
        }
    }
}
