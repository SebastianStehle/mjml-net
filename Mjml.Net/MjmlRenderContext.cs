using System.Xml;
using Mjml.Net.Components;
using Mjml.Net.Internal;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext
    {
        private readonly GlobalContext context = new GlobalContext();
        private readonly RenderStack<TransitiveContext> contextStack = new RenderStack<TransitiveContext>();
        private readonly ValidationErrors errors = new ValidationErrors();
        private MjmlOptions options;
        private MjmlRenderer renderer;
        private IValidator? validator;

        public MjmlRenderContext()
        {
        }

        public MjmlRenderContext(MjmlRenderer renderer, MjmlOptions options = default)
        {
            Setup(renderer,  options);
        }

        public void Setup(MjmlRenderer renderer, MjmlOptions options = default)
        {
            this.renderer = renderer;
            this.options = options;

            validator = options.Validator;
        }

        internal void Clear()
        {
            contextStack.Clear();
            context.Clear();
            errors.Clear();
            renderer = null!;

            ClearRenderData();
        }

        public List<ValidationError> Validate()
        {
            validator?.Complete(errors);

            return errors.ToList();
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
            var reader = parentReader.ReadSubtree();

            var component = renderer.CreateComponent(name);

            var currentLine = CurrentLine(reader);
            var currentColumn = CurrentColumn(reader);

            if (component == null)
            {
                errors.Add($"Invalid element '{name}'.",
                    CurrentLine(reader),
                    CurrentColumn(reader));
                return;
            }

            if (parent != null)
            {
                parent.AddChild(component);
            }

            var binder = new Binder(context, component, parent, name);

            validator?.ValidateComponent(component, errors,
                currentLine,
                currentColumn);

            reader.Read();

            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                binder.SetAttribute(reader.Name, reader.Value);

                validator?.ValidateAttribute(reader.Name, reader.Value, component, errors,
                    CurrentLine(reader),
                    CurrentColumn(reader));
            }

            if (component.Type == ComponentType.Text)
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

            if (component.Type == ComponentType.Raw)
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
