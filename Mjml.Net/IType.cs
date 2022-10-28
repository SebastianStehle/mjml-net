namespace Mjml.Net
{
    public interface IType
    {
        bool Validate(string value, ref ValidationContext context);
    }
}
