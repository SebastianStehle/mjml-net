namespace Mjml.Net.Types
{
    public class OneOfType : IType
    {
        private readonly IType[] units;

        public IReadOnlyCollection<Unit> Units => units;

        public OneOfType(params IType[] units)
        {
            this.units = units;
        }

        public bool Validate(string value, ref ValidationContext context)
        {
            foreach (var unit in units)
            {
                if (unit.Validate(value, ref context))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
