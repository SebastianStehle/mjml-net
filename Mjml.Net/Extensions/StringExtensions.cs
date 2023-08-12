using System.Globalization;

namespace Mjml.Net.Extensions;

public static class StringExtensions
{
    private static readonly char[] TrimChars = { ' ', '\n', '\r' };

    public static string ToInvariantString(this double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ToInvariantString(this int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static ReadOnlySpan<char> TrimInputStart(this ReadOnlySpan<char> source)
    {
        return source.TrimStart(TrimChars);
    }

    public static ReadOnlySpan<char> TrimInputEnd(this ReadOnlySpan<char> source)
    {
        return source.TrimEnd(TrimChars);
    }
}
