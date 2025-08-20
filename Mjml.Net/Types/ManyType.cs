namespace Mjml.Net.Types;

public sealed class ManyType(IType unit, int min, int max) : IType
{
    public IType Unit => unit;

    public int Min => min;

    public int Max => max;

    public bool Validate(string value, ref ValidationContext context)
    {
        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < min || parts.Length > max)
        {
            return false;
        }

        foreach (var part in parts)
        {
            if (!unit.Validate(part, ref context))
            {
                return false;
            }
        }

        return true;
    }
}
