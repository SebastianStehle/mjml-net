namespace Mjml.Net.Validators;

public sealed class StrictValidator : ValidatorBase
{
    public static readonly StrictValidator Instance = new StrictValidator();

    private StrictValidator()
    {
    }

    public override void AttributeValue(string name, string value, IComponent component, IType type, ValidationErrors errors, ref ValidationContext context)
    {
        if (!type.Validate(value, ref context))
        {
            errors.Add($"'{value}' is not a valid attribute '{name}' of '{component.ComponentName}'.",
                ValidationErrorType.InvalidAttribute,
                context.Position);
        }
    }
}
