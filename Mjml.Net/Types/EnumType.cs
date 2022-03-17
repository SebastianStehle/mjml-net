namespace Mjml.Net.Types
{
    public sealed class EnumType : IType
    {
        private readonly HashSet<string> allowedValues;

        public EnumType(params string[] values)
        {
            allowedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        }

        public bool Validate(string value)
        {
            return allowedValues.Contains(value);
        }
    }
}
