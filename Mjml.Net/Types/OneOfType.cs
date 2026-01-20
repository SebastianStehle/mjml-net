namespace Mjml.Net.Types;

public sealed class OneOfType(params IType[] units) : IType
{
    private readonly List<IType> units = [.. units];

    public IReadOnlyCollection<IType> Units => units;

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
