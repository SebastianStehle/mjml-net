namespace Mjml.Net
{
    public interface IValidator
    {
        void ValidateAttribute(string name, string value, IComponent component, ValidationErrors errors, int? line, int? column);

        void ValidateComponent(IComponent component, ValidationErrors errors, int? line, int? column);

        void Complete(ValidationErrors errors);
    }
}
