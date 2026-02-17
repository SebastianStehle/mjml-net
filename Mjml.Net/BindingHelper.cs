namespace Mjml.Net;

public static class BindingHelper
{
    public static string MakeLowerEndpoint(string breakpoint)
    {
        var (value, unit) = UnitParser.Parse(breakpoint);

        if (unit == Unit.Pixels)
        {
            return $"{value - 1}px";
        }

        return breakpoint;
    }

    public static (string? Top, string? Right, string? Bottom, string? Left) ParseShorthandBorder(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return (null, null, null, null);
        }

        return (value, value, value, value);
    }

    public static (string? Top, string? Right, string? Bottom, string? Left) ParseShorthandValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return (null, null, null, null);
        }

#if NET8_0_OR_GREATER
        // Optimize: Use stackalloc to avoid heap allocation for .NET 8+
        Span<Range> ranges = stackalloc Range[4];
        var span = value.AsSpan();
        var count = span.Split(ranges, ' ', StringSplitOptions.RemoveEmptyEntries);

        string? t = null;
        string? r = null;
        string? b = null;
        string? l = null;

        switch (count)
        {
            case 1:
                t = value[ranges[0]];
                r = value[ranges[0]];
                b = value[ranges[0]];
                l = value[ranges[0]];
                break;
            case 2:
                t = value[ranges[0]];
                r = value[ranges[1]];
                b = value[ranges[0]];
                l = value[ranges[1]];
                break;
            case 3:
                t = value[ranges[0]];
                r = value[ranges[1]];
                b = value[ranges[2]];
                l = value[ranges[1]];
                break;
            case >= 4:
                t = value[ranges[0]];
                r = value[ranges[1]];
                b = value[ranges[2]];
                l = value[ranges[3]];
                break;
        }

        return (t, r, b, l);
#else
        // For .NET 6/7, use standard Split with StringSplitOptions
        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string? t = null;
        string? r = null;
        string? b = null;
        string? l = null;

        switch (parts.Length)
        {
            case 1:
                t = parts[0];
                r = parts[0];
                b = parts[0];
                l = parts[0];
                break;
            case 2:
                t = parts[0];
                r = parts[1];
                b = parts[0];
                l = parts[1];
                break;
            case 3:
                t = parts[0];
                r = parts[1];
                b = parts[2];
                l = parts[1];
                break;
            case >= 4:
                t = parts[0];
                r = parts[1];
                b = parts[2];
                l = parts[3];
                break;
        }

        return (t, r, b, l);
#endif
    }
}
