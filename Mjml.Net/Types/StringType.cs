namespace Mjml.Net.Types;

public sealed class StringType(bool isRequired) : IType
{
    public bool Validate(string value, ref ValidationContext context)
    {
        return !isRequired || !string.IsNullOrEmpty(value);
    }
}
