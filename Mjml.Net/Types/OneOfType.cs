namespace Mjml.Net.Types;

public sealed class OneOfType : IType
{
    private readonly List<IType> units;

    public IReadOnlyCollection<IType> Units => units;

    public OneOfType(params IType[] units)
    {
        this.units = units.ToList();
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
