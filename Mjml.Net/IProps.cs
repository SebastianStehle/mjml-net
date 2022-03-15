namespace Mjml.Net
{
    public interface IProps
    {
        AllowedAttributes GetFields();

        string? DefaultValue(string name);
    }
}
