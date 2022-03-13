using System.Globalization;

namespace Mjml.Net.Components
{
    public static class NodeExtensions
    {
        public static (double TotalWidth, double Borders, double Paddings, double Box) GetBoxWidths(this INode node, IHtmlRenderer renderer)
        {
            var width = renderer.GetContext("containerWidth");

            var containerWidth = width is double i ? i : 600;

            var paddings =
                node.GetShorthandAttributeValue("padding-right", "padding") +
                node.GetShorthandAttributeValue("padding-left", "padding");

            var borders =
                node.GetShorthandBorderValue("border-right") +
                node.GetShorthandBorderValue("border-left");

            return (containerWidth, borders, paddings, containerWidth - paddings - borders);
        }

        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static double GetAttributeNumber(this INode node, string property)
        {
            var value = node.GetAttribute(property);

            return ParseNumber(value);
        }

        public static string GetAttributeNumberOrAuto(this INode node, string property)
        {
            var value = node.GetAttribute(property);

            if (value == "auto")
            {
                return "auto";
            }

            return ParseNumber(value).ToInvariantString();
        }

        public static double GetShorthandBorderValue(this INode node, string property)
        {
            var rawValue =
                node.GetAttribute(property) ??
                node.GetAttribute("border");

            return ParseNumber(rawValue);
        }

        public static double GetShorthandAttributeValue(this INode node, string property, string fallback)
        {
            var rawValue = node.GetAttribute(property);

            if (!string.IsNullOrWhiteSpace(rawValue))
            {
                return ParseNumber(rawValue);
            }

            rawValue = node.GetAttribute(fallback);

            return ParseShorthandValue(property, rawValue);
        }

        private static double ParseShorthandValue(string property, string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return 0;
            }

            var parts = rawValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            string t;
            string r;
            string b;
            string l;

            switch (parts.Length)
            {
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
                case 4:
                    t = parts[0];
                    r = parts[1];
                    b = parts[2];
                    l = parts[3];
                    break;
                default:
                    return 0;
            }

            if (property.EndsWith("-top", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNumber(t);
            }

            if (property.EndsWith("-right", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNumber(r);
            }

            if (property.EndsWith("-bottom", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNumber(b);
            }

            if (property.EndsWith("-left", StringComparison.OrdinalIgnoreCase))
            {
                return ParseNumber(l);
            }

            return 0;
        }

        private static double ParseNumber(string? rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return 0;
            }

            var span = rawValue.AsSpan().Trim();

            for (var i = 0; i < span.Length; i++)
            {
                if (!char.IsNumber(span[i]))
                {
                    span = span[..i];
                    break;
                }
            }

            if (span.Length == 0)
            {
                return 0;
            }

            double.TryParse(span, NumberStyles.Any, CultureInfo.InvariantCulture, out var temp);

            return temp;
        }
    }
}
