using System.Xml;
using Mjml.Net.Internal;
using Mjml.Net.Validators;

namespace Mjml.Net
{
    public sealed partial class MjmlRenderContext : INode
    {
        private static readonly char[] TrimChars = { ' ', '\n', '\r' };
        private readonly GlobalData globalData = new GlobalData();
        private readonly Dictionary<string, Dictionary<string, string>> attributesByName = new Dictionary<string, Dictionary<string, string>>(10);
        private readonly Dictionary<string, Dictionary<string, string>> attributesByClass = new Dictionary<string, Dictionary<string, string>>(10);
        private readonly RenderStack<ComponentContext> contextStack = new RenderStack<ComponentContext>();
        private readonly ValidationErrors errors = new ValidationErrors();
        private IComponent? currentComponent;
        private MjmlOptions options;
        private MjmlRenderer renderer;
        private IValidator? validator;
        private string? currentElement;
        private string[]? currentClasses;

        public INode Node
        {
            get => this;
        }

        public IComponent Component
        {
            get => currentComponent!;
        }

        public GlobalData GlobalData
        {
            get => globalData;
        }

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
            globalData.Clear();
            attributesByClass.Clear();
            attributesByName.Clear();
            contextStack.Clear();
            currentClasses = null;
            currentComponent = null;
            currentElement = null;
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
                        ReadElement(reader.Name, reader, default);
                        break;
                }
            }
        }

        private void ReadElement(string name, XmlReader parentReader, ChildOptions childOptions)
        {
            var reader = parentReader.ReadSubtree();

            var newContext = new ComponentContext(contextStack.Current, reader, childOptions);

            childOptions.ChildContext?.Invoke(newContext);

            contextStack.Push(newContext);

            currentElement = name;
            currentClasses = null;
            currentComponent = renderer.GetComponent(currentElement);

            var currentLine = CurrentLine(reader);
            var currentColumn = CurrentColumn(reader);

            if (currentComponent == null)
            {
                errors.Add($"Invalid element '{currentElement}'.",
                    CurrentLine(reader),
                    CurrentColumn(reader));
                return;
            }

            if (validator != null)
            {
                validator.ValidateComponent(currentComponent!, errors,
                    CurrentLine(reader),
                    CurrentColumn(reader));

                for (var i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);

                    validator.ValidateAttribute(reader.Name, reader.Value, currentComponent!, errors,
                        CurrentLine(reader),
                        CurrentColumn(reader));
                }
            }

            reader.Read();

            var childRenderer = contextStack.Current?.Options.Renderer;

            if (childRenderer != null)
            {
                childRenderer(this);
            }
            else
            {
                currentComponent!.Render(this, this);
            }

            contextStack.Pop();

            reader.Close();
        }

        public string? GetAttribute(string name, bool withoutDefaults = false)
        {
            if (Reader.MoveToAttribute(name))
            {
                return Reader.Value;
            }

            if (attributesByName.TryGetValue(name, out var byType))
            {
                if (byType.TryGetValue(currentElement!, out var attribute))
                {
                    return attribute;
                }

                if (byType.TryGetValue(Constants.All, out attribute))
                {
                    return attribute;
                }
            }

            if (attributesByClass.Count > 0)
            {
                if (currentClasses == null)
                {
                    if (Reader.MoveToAttribute(Constants.MjClass))
                    {
                        currentClasses = Reader.Value.Split(' ');
                    }
                    else
                    {
                        currentClasses = Array.Empty<string>();
                    }
                }

                foreach (var className in currentClasses)
                {
                    if (attributesByClass.TryGetValue(className, out var byName))
                    {
                        if (byName.TryGetValue(name, out var attribute))
                        {
                            return attribute;
                        }
                    }
                }
            }

            if (!withoutDefaults)
            {
                return currentComponent?.Props?.DefaultValue(name);
            }

            return null;
        }

        public object? Set(string name, object? value)
        {
            return contextStack.Current?.Set(name, value);
        }

        public object? Get(string name)
        {
            return contextStack.Current?.Get(name);
        }

        public string? GetText()
        {
            var reader = Reader;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    return Reader.Value.Trim(TrimChars);
                }
            }

            return null;
        }

        public void SetGlobalData(string name, object value, bool skipIfAdded = true)
        {
            var type = value.GetType();

            if (skipIfAdded && globalData.ContainsKey((type, name)))
            {
                return;
            }

            globalData[(type, name)] = value;
        }

        public void SetTypeAttribute(string name, string type, string value)
        {
            if (!attributesByName.TryGetValue(name, out var attributes))
            {
                attributes = new Dictionary<string, string>();

                attributesByName[name] = attributes;
            }

            attributes[type] = value;
        }

        public void SetClassAttribute(string name, string className, string value)
        {
            if (!attributesByClass.TryGetValue(className, out var attributes))
            {
                attributes = new Dictionary<string, string>();

                attributesByClass[className] = attributes;
            }

            attributes[name] = value;
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
