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
                errors.Add("Document must have 'mj-body' tag.", ValidationErrorType.Other);
            }

            return errors;
        }

        public void Attribute(string name, string value, IComponent component, ref ValidationContext context)
        {
            var allowedAttributes = component.AllowedFields;

            if (allowedAttributes == null)
            {
                return;
            }

            if (!allowedAttributes.TryGetValue(name, out var attribute))
            {
                errors.Add($"'{name}' is not a valid attribute of '{component.ComponentName}'.",
                    ValidationErrorType.UnknownAttribute,
                    context.LineNumber,
                    context.LinePosition);
            }
            else if (validateAttributeValue && !attribute.Validate(value, ref context))
            {
                errors.Add($"'{value}' is not a valid attribute '{name}' of '{component.ComponentName}'.",
                    ValidationErrorType.InvalidAttribute,
                    context.LineNumber,
                    context.LinePosition);
            }
        }

        public void BeforeComponent(IComponent component, ref ValidationContext context)
        {
            var name = component.ComponentName;

            if (name == "mj-body")
            {
                hasBody = true;
            }

            if (component.AllowedParents == null || component.AllowedParents.Count == 0)
            {
                if (componentStack.Count > 0)
                {
                    errors.Add($"'{name}' must be the root tag.",
                        ValidationErrorType.InvalidParent,
                        context.LineNumber,
                        context.LinePosition);
                }
            }
            else
            {
                if (!componentStack.TryPeek(out var previous))
                {
                    errors.Add($"'{name}' cannot be the root tag.",
                        ValidationErrorType.InvalidParent,
                        context.LineNumber,
                        context.LinePosition);
                }
                else if (component.AllowedParents != null)
                {
                    if (!component.AllowedParents.Contains(previous))
                    {
                        errors.Add($"'{name}' must be child of '{string.Join(", ", component.AllowedParents)}'.",
                            ValidationErrorType.InvalidParent,
                            context.LineNumber,
                            context.LinePosition);
                    }
                }
            }

            componentStack.Push(component.ComponentName);
        }

        public void AfterComponent(IComponent component, ref ValidationContext context)
        {
            componentStack.TryPop(out var _);
        }
    }
}
