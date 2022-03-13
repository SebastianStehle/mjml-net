using System.Globalization;

namespace Mjml.Net.Components
{
    public static class NodeExtensions
    {
        public static (double TotalWidth, double Borders, double Paddings, double Box) GetBoxWidths(this INode node, IHtmlRenderer renderer)
        {
            var containerWidth = renderer.GetContainerWidth();

            var paddings =
                node.GetShorthandAttributeValue("padding-right", "padding") +
                node.GetShorthandAttributeValue("padding-left", "padding");

            var borders =
                node.GetShorthandBorderValue("border-right") +
                node.GetShorthandBorderValue("border-left");

            return (containerWidth, borders, paddings, containerWidth - paddings - borders);
        }

        public static double GetContainerWidth(this IHtmlRenderer renderer)
        {
            var width = renderer.GetContext("containerWidth");

            return width is double i ? i : 600;
        }

        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static double GetAttributeNumber(this INode node, string property)
        {
            var value = node.GetAttribute(property);

            return UnitParser.Parse(value).Value;
        }

        public static string GetAttributeNumberOrAuto(this INode node, string property)
        {
            var value = node.GetAttribute(property);

            if (value == "auto")
            {
                return "auto";
            }

            return UnitParser.Parse(value).Value.ToInvariantString();
        }

        public static double GetShorthandBorderValue(this INode node, string property)
        {
            var rawValue =
                node.GetAttribute(property) ??
                node.GetAttribute("border");

            return UnitParser.Parse(rawValue).Value;
        }

        public static double GetShorthandAttributeValue(this INode node, string property, string fallback)
        {
            var rawValue = node.GetAttribute(property);

            if (!string.IsNullOrWhiteSpace(rawValue))
            {
                return UnitParser.Parse(rawValue).Value;
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
                return UnitParser.Parse(t).Value;
            }

            if (property.EndsWith("-right", StringComparison.OrdinalIgnoreCase))
            {
                return UnitParser.Parse(r).Value;
            }

            if (property.EndsWith("-bottom", StringComparison.OrdinalIgnoreCase))
            {
                return UnitParser.Parse(b).Value;
            }

            if (property.EndsWith("-left", StringComparison.OrdinalIgnoreCase))
            {
                return UnitParser.Parse(l).Value;
            }

            return 0;
        }
    }
}
