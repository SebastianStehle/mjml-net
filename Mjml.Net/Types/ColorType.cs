using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Mjml.Net.Types;

public sealed partial class ColorType : IType
{
    private static readonly HashSet<ReadOnlyMemory<char>> Colors = new[]
    {
        "aliceblue",
        "antiquewhite",
        "aqua",
        "aquamarine",
        "azure",
        "beige",
        "bisque",
        "black",
        "blanchedalmond",
        "blue",
        "blueviolet",
        "brown",
        "burlywood",
        "cadetblue",
        "chartreuse",
        "chocolate",
        "coral",
        "cornflowerblue",
        "cornsilk",
        "crimson",
        "cyan",
        "darkblue",
        "darkcyan",
        "darkgoldenrod",
        "darkgray",
        "darkgreen",
        "darkgrey",
        "darkkhaki",
        "darkmagenta",
        "darkolivegreen",
        "darkorange",
        "darkorchid",
        "darkred",
        "darksalmon",
        "darkseagreen",
        "darkslateblue",
        "darkslategray",
        "darkslategrey",
        "darkturquoise",
        "darkviolet",
        "deeppink",
        "deepskyblue",
        "dimgray",
        "dimgrey",
        "dodgerblue",
        "firebrick",
        "floralwhite",
        "forestgreen",
        "fuchsia",
        "gainsboro",
        "ghostwhite",
        "gold",
        "goldenrod",
        "gray",
        "green",
        "greenyellow",
        "grey",
        "honeydew",
        "hotpink",
        "indianred",
        "indigo",
        "inherit",
        "ivory",
        "khaki",
        "lavender",
        "lavenderblush",
        "lawngreen",
        "lemonchiffon",
        "lightblue",
        "lightcoral",
        "lightcyan",
        "lightgoldenrodyellow",
        "lightgray",
        "lightgreen",
        "lightgrey",
        "lightpink",
        "lightsalmon",
        "lightseagreen",
        "lightskyblue",
        "lightslategray",
        "lightslategrey",
        "lightsteelblue",
        "lightyellow",
        "lime",
        "limegreen",
        "linen",
        "magenta",
        "maroon",
        "mediumaquamarine",
        "mediumblue",
        "mediumorchid",
        "mediumpurple",
        "mediumseagreen",
        "mediumslateblue",
        "mediumspringgreen",
        "mediumturquoise",
        "mediumvioletred",
        "midnightblue",
        "mintcream",
        "mistyrose",
        "moccasin",
        "navajowhite",
        "navy",
        "oldlace",
        "olive",
        "olivedrab",
        "orange",
        "orangered",
        "orchid",
        "palegoldenrod",
        "palegreen",
        "paleturquoise",
        "palevioletred",
        "papayawhip",
        "peachpuff",
        "peru",
        "pink",
        "plum",
        "powderblue",
        "purple",
        "rebeccapurple",
        "red",
        "rosybrown",
        "royalblue",
        "saddlebrown",
        "salmon",
        "sandybrown",
        "seagreen",
        "seashell",
        "sienna",
        "silver",
        "skyblue",
        "slateblue",
        "slategray",
        "slategrey",
        "snow",
        "springgreen",
        "steelblue",
        "tan",
        "teal",
        "thistle",
        "tomato",
        "transparent",
        "turquoise",
        "violet",
        "wheat",
        "white",
        "whitesmoke",
        "yellow",
        "yellowgreen",
    }.Select(x => x.AsMemory()).ToHashSet(new Comparer());

    private sealed class Comparer : IEqualityComparer<ReadOnlyMemory<char>>
    {
        public bool Equals(ReadOnlyMemory<char> lhs, ReadOnlyMemory<char> rhs)
        {
            return rhs.Span.Equals(rhs.Span, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
        {
            return string.GetHashCode(obj.Span, StringComparison.OrdinalIgnoreCase);
        }
    }

#if NET7_0_OR_GREATER
    private static readonly Regex Rgba = RgbaFactory();
    private static readonly Regex Rgb = RgbFactory();
    private static readonly Regex Hex = HexFactory();

    [GeneratedRegex("^rgba\\(\\d{1,3},\\s?\\d{1,3},\\s?\\d{1,3},\\s?\\d(\\.\\d{1,3})?\\)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex RgbaFactory();

    [GeneratedRegex("^rgb\\(\\d{1,3},\\s?\\d{1,3},\\s?\\d{1,3}\\)?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex RgbFactory();

    [GeneratedRegex("^#([0-9a-fA-F]{3}){1,2}?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    private static partial Regex HexFactory();
#else
    private static readonly Regex Rgba = new Regex(@"^rgba\(\d{1,3},\s?\d{1,3},\s?\d{1,3},\s?\d(\.\d{1,3})?\)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    private static readonly Regex Rgb = new Regex(@"^rgb\(\d{1,3},\s?\d{1,3},\s?\d{1,3}\)?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
    private static readonly Regex Hex = new Regex(@"^#([0-9a-fA-F]{3}){1,2}?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
#endif

    public bool Validate(string value, ref ValidationContext context)
    {
#if NET7_0_OR_GREATER
        var trimmed = value.AsMemory().Trim();

        if (Colors.Contains(trimmed))
        {
            return true;
        }

        return Rgba.IsMatch(trimmed.Span) || Rgb.IsMatch(trimmed.Span) || Hex.IsMatch(trimmed.Span);
#else
        var trimmed = value.Trim();

        if (Colors.Contains(trimmed.AsMemory()))
        {
            return true;
        }

        return Rgba.IsMatch(trimmed) || Rgb.IsMatch(trimmed) || Hex.IsMatch(trimmed);
#endif
    }

    public string Coerce(string value)
    {
        var trimmed = value.AsSpan().Trim();

        if (trimmed.Length == 4 && trimmed[0] == '#')
        {
            return new string(new char[]
            {
                trimmed[0],
                trimmed[1],
                trimmed[1],
                trimmed[2],
                trimmed[2],
                trimmed[3],
                trimmed[3]
            });
        }

        return value;
    }
}
