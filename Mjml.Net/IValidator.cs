namespace Mjml.Net
{
    public interface IValidator
    {
        void Attribute(string name, string value, IComponent component, ValidationErrors errors, int? line, int? column)
        {
        }

        void BeforeComponent(IComponent component, ValidationErrors errors, int? line, int? column)
        {
        }

        void AfterComponent(IComponent component, ValidationErrors errors, int? line, int? column)
        {
        }

        void Complete(ValidationErrors errors)
        {
        }
    }
}
