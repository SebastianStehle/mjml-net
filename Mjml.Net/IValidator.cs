namespace Mjml.Net
{
    public interface IValidator
    {
        void Attribute(string name, string value, IComponent component, int? line, int? column);

        void BeforeComponent(IComponent component, int? line, int? column);

        void AfterComponent(IComponent component, int? line, int? column);

        ValidationErrors Complete();
    }
}
