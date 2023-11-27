namespace Mjml.Net;

public interface IType
{
    bool Validate(string value, ref ValidationContext context);

    public string Coerce(string value)
    {
        return value;
    }
}
