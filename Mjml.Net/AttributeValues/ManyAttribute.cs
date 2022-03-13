namespace Mjml.Net.AttributeValues
{
    public sealed class ManyAttribute : IAttribute
    {
        private readonly IAttribute unit;
        private readonly int min;
        private readonly int max;

        public ManyAttribute(IAttribute unit, int min, int max)
        {
            this.unit = unit;
            this.min = min;
            this.max = max;
        }

        public bool Validate(string value)
        {
            var parts = value.Split(" ");

            return parts.Length >= min && parts.Length <= max && parts.All(x => unit.Validate(x));
        }
    }
}
