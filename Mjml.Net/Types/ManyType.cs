namespace Mjml.Net.Types
{
    public sealed class ManyType : IType
    {
        private readonly IType unit;
        private readonly int min;
        private readonly int max;

        public ManyType(IType unit, int min, int max)
        {
            this.unit = unit;
            this.min = min;
            this.max = max;
        }

        public bool Validate(string value)
        {
            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return parts.Length >= min && parts.Length <= max && parts.All(unit.Validate);
        }
    }
}
