namespace Mjml.Net.Validators
{
    public sealed class SoftValidator : IValidator
    {
        public static readonly SoftValidator Instance = new SoftValidator();

        private SoftValidator()
        {
        }

        public void Complete(ValidationErrors errors)
        {
        }

        public void ValidateAttribute(string name, string value, IComponent component, ValidationErrors errors, int? line, int? column)
        {
        }

        public void ValidateComponent(IComponent component, ValidationErrors errors, int? line, int? column)
        {
        }
    }
}
