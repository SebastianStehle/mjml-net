namespace Mjml.Net
{
    public interface IType
    {
        bool Validate(string value);

        string Coerce(string value)
        {
            return value;
        }
    }
}
