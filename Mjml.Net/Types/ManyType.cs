namespace Mjml.Net.Types;

public sealed class ManyType(IType unit, int min, int max) : IType
{
    public IType Unit => unit;

    public int Min => min;

    public int Max => max;

    public bool Validate(string value, ref ValidationContext context)
    {
#if NET8_0_OR_GREATER
        // Optimize: Use stackalloc to avoid heap allocation (max is typically 4)
        // Allocate one extra slot to detect when count exceeds max
        // Limit to 17 (16 + 1) as a reasonable stack allocation safety threshold
        Span<Range> ranges = stackalloc Range[Math.Min(max + 1, 17)];
        var span = value.AsSpan();
        var count = span.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

        if (count < min || count > max)
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            var part = value[ranges[i]];
            if (!unit.Validate(part, ref context))
            {
                return false;
            }
        }

        return true;
#else
        // For .NET 6/7, use standard Split
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
#endif
    }
}
