namespace Mjml.Net.Types
{
    public sealed class NumberType : IType
    {
        private readonly Unit[] units;

        public NumberType(params Unit[] units)
        {
            this.units = units;
        }

        public bool Validate(string value)
        {
            if (value == "0")
            {
                return true;
            }

            var (_, unit) = UnitParser.Parse(value);

            return units.Contains(unit);
        }
    }
}
