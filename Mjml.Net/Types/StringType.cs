namespace Mjml.Net.Types;

public sealed class StringType : IType
{
    public bool Validate(string value, ref ValidationContext context)
    {
        return true;
    }
}
