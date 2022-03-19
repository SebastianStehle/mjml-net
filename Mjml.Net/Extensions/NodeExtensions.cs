using System.Globalization;
using Mjml.Net.Components;

namespace Mjml.Net.Extensions
{
    public static class NodeExtensions
    {
        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static IElementStyleWriter StyleIf(this IElementStyleWriter writer, string name, bool condition, string? value)
        {
            if (condition)
            {
                writer.Style(name, value);
            }

            return writer;
        }

        public static IElementStyleWriter StyleIf(this IElementStyleWriter writer, string name, bool condition, double value, string unit)
        {
            if (condition)
            {
                writer.Style(name, value, unit);
            }

            return writer;
        }

        public static string GetNumberOrAuto(this string? value)
        {
            if (value == "auto")
            {
                return "auto";
            }

            return UnitParser.Parse(value).Value.ToInvariantString();
        }
    }
}
