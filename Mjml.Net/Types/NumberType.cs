namespace Mjml.Net.Types;

public sealed class NumberType : IType
{
    private readonly Unit[] units;

    public IReadOnlyCollection<Unit> Units => units;

    public NumberType(params Unit[] units)
    {
        this.units = units;
    }

    public bool Validate(string value, ref ValidationContext context)
    {
        var trimmed = value.AsSpan().Trim();

        if (trimmed.Length == 1 && trimmed[0] == '0')
        {
            return true;
        }

        var (_, unit) = UnitParser.Parse(value);

        return units.Contains(unit);
    }
}
