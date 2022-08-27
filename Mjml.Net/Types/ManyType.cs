namespace Mjml.Net.Types
{
    public class ManyType : IType
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

        public bool Validate(string value, ref ValidationContext context)
        {
            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < min || parts.Length > max)
            {
                return false;
            }

            foreach (var part in parts)
            {
                if (!unit.Validate(value, ref context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
