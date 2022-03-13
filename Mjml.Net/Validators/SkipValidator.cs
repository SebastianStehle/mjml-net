namespace Mjml.Net.Validators
{
    public sealed class SkipValidator : IValidator
    {
        public static readonly SkipValidator Instance = new SkipValidator();

        private SkipValidator()
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
