namespace Mjml.Net.Validators
{
    public abstract class ValidatorBase : IValidator
    {
        private readonly bool validateAttributeValue;
        private readonly Stack<string> componentStack = new Stack<string>();
        private readonly ValidationErrors errors = new ValidationErrors();
        private bool hasBody;

        protected ValidatorBase(bool validateAttributeValue)
        {
            this.validateAttributeValue = validateAttributeValue;
        }

        public ValidationErrors Complete()
        {
            if (!hasBody)
            {
                errors.Add("Document must have 'mj-body' tag.");
            }

            return errors;
        }

        public void Attribute(string name, string value, IComponent component, int? line, int? column)
        {
            var allowedAttributes = component.Props?.GetFields();

            if (allowedAttributes == null)
            {
                return;
            }

            if (!allowedAttributes.TryGetValue(name, out var attribute))
            {
                errors.Add($"'{name}' is not a valid attribute of '{component.Name}'.", line, column);
            }
            else if (validateAttributeValue && !attribute.Validate(value))
            {
                errors.Add($"'{value}' is not a valid attribute '{name}' of '{component.Name}'.", line, column);
            }
        }

        public void BeforeComponent(IComponent component, int? line, int? column)
        {
            var name = component.Name;

            if (name == "mj-body")
            {
                hasBody = true;
            }

            if (component.AllowedAsDescendant == null && component.AllowedAsChild == null)
            {
                if (componentStack.Count > 0)
                {
                    errors.Add($"'{name}' must be the root tag.", line, column);
                }
            }
            else
            {
                if (!componentStack.TryPeek(out var previous))
                {
                    errors.Add($"'{name}' cannot be the root tag.", line, column);
                }
                else if (component.AllowedAsChild != null)
                {
                    if (!component.AllowedAsChild.Contains(previous))
                    {
                        errors.Add($"'{name}' must be child of '{string.Join(", ", component.AllowedAsChild)}'.", line, column);
                    }
                }
                else if (component.AllowedAsDescendant != null)
                {
                    if (component.AllowedAsDescendant.All(x => !componentStack.Contains(x)))
                    {
                        errors.Add($"'{name}' must be descendant of '{string.Join(", ", component.AllowedAsDescendant)}'.", line, column);
                    }
                }
            }

            componentStack.Push(component.Name);
        }

        public void AfterComponent(IComponent component, int? line, int? column)
        {
            componentStack.TryPop(out var _);
        }
    }
}
