namespace Mjml.Net.AttributeValues
{
    public sealed class OneOfAttribute : IAttribute
    {
        private readonly IAttribute[] units;

        public OneOfAttribute(params IAttribute[] units)
        {
            this.units = units;
        }

        public bool Validate(string value)
        {
            return units.Any(x => x.Validate(value));
        }
    }
}
