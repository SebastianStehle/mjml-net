namespace Mjml.Net.Validators
{
    public sealed class StrictValidator : IValidator
    {
        public static readonly StrictValidator Instance = new StrictValidator();

        private StrictValidator()
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
