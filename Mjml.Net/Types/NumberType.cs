using Mjml.Net.Components;

namespace Mjml.Net.Types
{
    public sealed class NumberType : IType
    {
        private readonly UnitType[] units;

        public NumberType(params UnitType[] units)
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
