namespace Mjml.Net.Types;

public class EnumType(bool isOptional, params string[] values) : IType
{
    private readonly HashSet<string> allowedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

    public bool IsOptional => isOptional;

    public IReadOnlySet<string> AllowedValues => allowedValues;

    public bool Validate(string value, ref ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(value) && isOptional)
        {
            return true;
        }

        return allowedValues.Contains(value);
    }
}
