namespace Mjml.Net.Types
{
    public class EnumType : IType
    {
        private readonly HashSet<string> allowedValues;
        private readonly bool isOptional;

        public EnumType(bool isOptional, params string[] values)
        {
            allowedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);

            this.isOptional = isOptional;
        }

        public bool Validate(string value, ref ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(value) && isOptional)
            {
                return true;
            }

            return allowedValues.Contains(value);
        }
    }
}
