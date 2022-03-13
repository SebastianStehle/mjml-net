using Mjml.Net.Components;

namespace Mjml.Net.AttributeValues
{
    public sealed class NumberAttribute : IAttribute
    {
        private readonly Unit[] units;

        public NumberAttribute(params Unit[] units)
        {
            this.units = units;
        }

        public bool Validate(string value)
        {
            var (_, unit) = UnitParser.Parse(value);

            return units.Contains(unit);
        }
    }
}
