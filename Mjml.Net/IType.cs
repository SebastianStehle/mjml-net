namespace Mjml.Net;

public interface IType
{
    bool Validate(string value, ref ValidationContext context);

    string Coerce(string value)
    {
        return value;
    }
}
