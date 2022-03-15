﻿using System.Xml;
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
        private readonly Dictionary<string, string> currentAttributes = new Dictionary<string, string>(10);
        private readonly RenderStack<ComponentContext> contextStack = new RenderStack<ComponentContext>();
        private readonly ValidationErrors errors = new ValidationErrors();
        private IComponent? currentComponent;
        private MjmlOptions options;
        private MjmlRenderer renderer;
        private IValidator validator;
        private string? currentText;
        private string? currentElement;
        private string[]? currentClasses;

        public IComponent Component => currentComponent!;

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

            validator = options.Validator ?? SoftValidator.Instance;
        }

        internal void Clear()
        {
            attributesByClass.Clear();
            attributesByName.Clear();
            contextStack.Clear();
            currentAttributes.Clear();
            currentClasses = null;
            currentComponent = null;
            currentElement = null;
            currentText = null;
            errors.Clear();
            globalData.Clear();
            renderer = null!;

            ClearRenderData();
        }

        public void Validate()
        {
            validator.Complete(errors);

            if (errors.Any())
            {
                // Create a copy because the error list could be reused.
                throw new ValidationException(errors.ToList());
            }
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

            currentComponent?.AddToChildContext(newContext, contextStack.Current!, this);
            contextStack.Push(newContext);

            currentElement = name;
            currentClasses = null;
            currentText = null;
            currentAttributes.Clear();
            currentComponent = renderer.GetComponent(currentElement);

            var currentLine = CurrentLine(reader);
            var currentColumn = CurrentColumn(reader);

            if (currentComponent == null)
            {
                errors.Add($"Invalid element '{currentElement}'.",
                    CurrentLine(reader),
                    CurrentColumn(reader));
                Validate();
            }

            validator.ValidateComponent(currentComponent!, errors,
                CurrentLine(reader),
                CurrentColumn(reader));

            reader.Read();

            for (var i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);

                currentAttributes[reader.Name] = reader.Value;

                validator.ValidateAttribute(reader.Name, reader.Value, currentComponent!, errors,
                    CurrentLine(reader),
                    CurrentColumn(reader));
            }

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

        public string? GetAttribute(string name, string? fallback = null)
        {
            string ProvideValue(string name, string value)
            {
                if (currentComponent?.AllowedAttributes?.TryGetValue(name, out var attribute) == true)
                {
                    return attribute.Coerce(value);
                }

                return value;
            }

            if (currentAttributes.TryGetValue(name, out var attribute))
            {
                return ProvideValue(name, attribute);
            }

            if (attributesByName.TryGetValue(name, out var byType))
            {
                if (byType.TryGetValue(currentElement!, out attribute))
                {
                    return ProvideValue(name, attribute);
                }

                if (byType.TryGetValue(Constants.All, out attribute))
                {
                    return ProvideValue(name, attribute);
                }
            }

            if (attributesByClass.Count > 0)
            {
                if (currentClasses == null)
                {
                    currentClasses = currentAttributes.GetValueOrDefault(Constants.MjClass)?.Split() ?? Array.Empty<string>();
                }

                foreach (var className in currentClasses)
                {
                    if (attributesByClass.TryGetValue(className, out var byName))
                    {
                        if (byName.TryGetValue(name, out attribute))
                        {
                            return ProvideValue(name, attribute);
                        }
                    }
                }
            }

            var defaultAttributes = currentComponent?.DefaultAttributes;

            if (defaultAttributes != null && defaultAttributes.TryGetValue(name, out attribute))
            {
                return ProvideValue(name, attribute);
            }

            return fallback;
        }

        public object? Set(string name, object? value)
        {
            return contextStack.Current?.Set(name, value);
        }

        public object? Get(string name)
        {
            return contextStack.Current?.Get(name);
        }

        public string? GetContent()
        {
            var reader = contextStack.Current!.Reader;

            if (currentText == null)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        currentText = reader.Value.Trim(TrimChars);
                        break;
                    }
                }

                currentText ??= string.Empty;
            }

            return currentText;
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
