namespace Mjml.Net.Types;

public sealed class StringType : IType
{
    private readonly bool isRequired;

    public StringType(bool isRequired)
    {
        this.isRequired = isRequired;
    }

    public bool Validate(string value, ref ValidationContext context)
    {
        return !isRequired || !string.IsNullOrEmpty(value);
    }
}
