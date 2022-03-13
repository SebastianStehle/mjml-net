namespace Mjml.Net.AttributeValues
{
    public sealed class EnumAttribute : IAttribute
    {
        private readonly HashSet<string> allowedValues;

        public EnumAttribute(params string[] values)
        {
            allowedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        }

        public bool Validate(string value)
        {
            return allowedValues.Contains(value);
        }
    }
}
