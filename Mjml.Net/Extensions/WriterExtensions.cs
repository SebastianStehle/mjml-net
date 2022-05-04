namespace Mjml.Net.Extensions
{
    public static class WriterExtensions
    {
        public static IHtmlStyleRenderer StyleIf(this IHtmlStyleRenderer renderer, string name, bool condition, string? value)
        {
            if (condition)
            {
                renderer.Style(name, value);
            }

            return renderer;
        }

        public static IHtmlStyleRenderer StyleIf(this IHtmlStyleRenderer renderer, string name, bool condition, double value, string unit)
        {
            if (condition)
            {
                renderer.Style(name, $"{value}{unit}");
            }

            return renderer;
        }

        public static IHtmlStyleRenderer StyleOrNone(this IHtmlStyleRenderer renderer, string name, double value)
        {
            if (double.IsNaN(value))
            {
                return renderer;
            }
            else
            {
                return renderer.Style(name, $"{value}px");
            }
        }

        public static IHtmlAttrRenderer AttrOrAuto(this IHtmlAttrRenderer renderer, string name, string? value)
        {
            if (value == "auto")
            {
                return renderer.Attr(name, "auto");
            }
            else
            {
                return renderer.Attr(name, $"{UnitParser.Parse(value).Value}");
            }
        }

        public static IHtmlClassRenderer Classes(this IHtmlClassRenderer renderer, string? classNames, string suffix)
        {
            if (string.IsNullOrEmpty(classNames))
            {
                return renderer;
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                renderer.Class(classNames);
                return renderer;
            }

            var span = classNames.AsSpan().Trim();

            while (span.Length > 0)
            {
                var index = span.IndexOf(' ');

                if (index > 0)
                {
                    renderer.Class($"{span[..index]}-{suffix}");

                    span = span[index..].Trim();
                }
                else
                {
                    break;
                }
            }

            if (span.Length > 0)
            {
                renderer.Class($"{span}-{suffix}");
            }

            return renderer;
        }
    }
}
