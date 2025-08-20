namespace Mjml.Net.Types;

public sealed class NumberType(params Unit[] units) : IType
{
    public IReadOnlyCollection<Unit> Units => units;

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
