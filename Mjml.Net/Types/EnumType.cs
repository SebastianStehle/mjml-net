namespace Mjml.Net.Types;

public class EnumType(bool isOptional, params string[] values) : IType
{
#if NET8_0_OR_GREATER
    // Optimize: Use FrozenSet on .NET 8+ for better lookup performance
    private readonly System.Collections.Frozen.FrozenSet<string> allowedValues =
        System.Collections.Frozen.FrozenSet.ToFrozenSet(values, StringComparer.OrdinalIgnoreCase);
#else
    private readonly HashSet<string> allowedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
#endif

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
