using System.Xml;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext
    {
        private readonly GlobalContext context = new GlobalContext();
        private readonly RenderStack<TransitiveContext> contextStack = new RenderStack<TransitiveContext>();
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
            this.mjmlOptions = mjmlOptions ?? new MjmlOptions();

            validator = this.mjmlOptions.ValidatorFactory?.Create();

            // Reuse the context and therefore do not set them over the properties.
            context.SetOptions(this.mjmlOptions);
        }

        internal void Clear()
        {
            contextStack.Clear();
            context.Clear();
            errors.Clear();
            mjmlRenderer = null!;

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

                binder.SetAttribute(reader.Name, reader.Value);

                validator?.Attribute(reader.Name, reader.Value, component,
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
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Text:
                            component.AddChild(reader.Value);
                            break;
                        case XmlNodeType.Element:
                            component.AddChild(reader.ReadOuterXml().Trim());

                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                component.AddChild(reader.Value);
                            }

                            break;
                    }
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
                    }
                }
            }

            reader.Close();

            if (parent == null)
            {
                component.Render(this, context);
            }

            validator?.AfterComponent(component,
                currentLine,
                currentColumn);
        }

        public object? Set(string name, object? value)
        {
            return contextStack.Current?.Set(name, value);
        }

        public object? Get(string name)
        {
            return contextStack.Current?.Get(name);
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
