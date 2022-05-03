using System.Xml;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext
    {
        private readonly GlobalContext context = new GlobalContext();
        private readonly ValidationErrors errors = new ValidationErrors();
        private readonly Binder binder;
        private MjmlOptions mjmlOptions;
        private MjmlRenderer mjmlRenderer;
        private IValidator? validator;

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

            // Just for convience.
            this.mjmlOptions = mjmlOptions ?? new MjmlOptions();

            // We have to create a new instance each time, because it could another factory.
            validator = this.mjmlOptions.ValidatorFactory?.Create();

            // Reuse the context and therefore do not set them over the constructor.
            context.SetOptions(this.mjmlOptions);
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

        public void Read(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ReadElement(reader.Name, reader, null);
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

            validator?.BeforeComponent(component,
                currentLine,
                currentColumn);

            reader.Read();

            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                var attributeName = reader.Name;
                var attributeValue = reader.Value;

                binder.SetAttribute(attributeName, attributeValue);

                validator?.Attribute(attributeName, attributeValue, component,
                    CurrentLine(reader),
                    CurrentColumn(reader));
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

                            if (reader.NodeType != XmlNodeType.Text)
                            {
                                Read();
                            }
                            break;
                        case XmlNodeType.Element:
                            component.AddChild(reader.ReadOuterXml().Trim());
                            Read();
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
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ReadElement(reader.Name, reader, component);
                            break;
                        case XmlNodeType.Comment when mjmlOptions.KeepComments:
                            ReadComment(reader, component);
                            break;
                    }
                }
            }

            component.AfterBind(context, reader);

            reader.Close();

            if (parent == null)
            {
                component.Measure(600, 0, 0);
                component.Render(this, context);
            }

            validator?.AfterComponent(component,
                currentLine,
                currentColumn);
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
