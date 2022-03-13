namespace Mjml.Net.AttributeValues
{
    public sealed class NumberAttribute : IAttribute
    {
        private readonly string[] units;

        public NumberAttribute(params string[] units)
        {
            this.units = units;
        }

        public bool Validate(string value)
        {
            throw new NotImplementedException();
        }
    }
}
