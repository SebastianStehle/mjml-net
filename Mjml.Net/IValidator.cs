namespace Mjml.Net
{
    public interface IValidator
    {
        void Attribute(string name, string value, IComponent component, ref ValidationContext context);

        void BeforeComponent(IComponent component, ref ValidationContext context);

        void AfterComponent(IComponent component, ref ValidationContext context);

        ValidationErrors Complete();
    }
}
