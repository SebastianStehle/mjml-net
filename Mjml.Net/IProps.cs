namespace Mjml.Net
{
    public interface IProps
    {
        AllowedAttributes GetFields();

        string? DefaultValue(string name);

        void Bind(INode node);
    }
}
