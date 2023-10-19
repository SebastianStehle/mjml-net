using Mjml.Net.Components;
using Mjml.Net.Components.Body;

namespace Mjml.Net.Validators;

public abstract class ValidatorBase : IValidator
{
    private readonly bool validateAttributeValue;

    protected ValidatorBase(bool validateAttributeValue)
    {
        this.validateAttributeValue = validateAttributeValue;
    }

    public void Attribute(string name, string value, IComponent component, ValidationErrors errors, ref ValidationContext context)
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
                context.Position);
        }
        else if (validateAttributeValue && !attribute.Validate(value, ref context))
        {
            errors.Add($"'{value}' is not a valid attribute '{name}' of '{component.ComponentName}'.",
                ValidationErrorType.InvalidAttribute,
                context.Position);
        }
    }

    public void Components(IComponent root, ValidationErrors errors, ref ValidationContext context)
    {
        if (root is RootComponent rootComponent)
        {
            if (!rootComponent.ChildNodes.Any(x => x is BodyComponent))
            {
                errors.Add("Document must have 'mj-body' tag.", ValidationErrorType.Other);
            }
        }
        else
        {
            errors.Add($"'{root.ComponentName}' cannot be the root tag.",
                ValidationErrorType.InvalidParent,
                root.Position);
        }

        void Validate(IComponent component, IComponent? parent)
        {
            if (parent != null && component.AllowedParents?.Contains(parent.ComponentName) == false)
            {
                errors.Add($"'{component.ComponentName}' must be child of '{string.Join(", ", component.AllowedParents)}', found '{parent.ComponentName}'.",
                    ValidationErrorType.InvalidParent,
                    component.Position);
            }

            foreach (var child in component.ChildNodes)
            {
                Validate(child, component);
            }
        }

        Validate(root, null);
    }
}
