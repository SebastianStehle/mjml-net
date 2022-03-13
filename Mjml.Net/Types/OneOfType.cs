namespace Mjml.Net.Types
{
    public sealed class OneOfType : IType
    {
        private readonly IType[] units;

        public OneOfType(params IType[] units)
        {
            this.units = units;
        }

        public bool Validate(string value)
        {
            return units.Any(x => x.Validate(value));
        }
    }
}
