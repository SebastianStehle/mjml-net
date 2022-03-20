using Mjml.Net.Components;

namespace Mjml.Net.Extensions
{
    public static class WriterExtensions
    {
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
                writer.Style(name, $"{value}{unit}");
            }

            return writer;
        }

        public static IElementHtmlWriter AttrOrAuto(this IElementHtmlWriter writer, string name, string? value)
        {
            if (value == "auto")
            {
                return writer.Attr(name, "auto");
            }
            else
            {
                return writer.Attr(name, $"{UnitParser.Parse(value).Value}");
            }
        }

        public static IElementClassWriter Classes(this IElementClassWriter writer, string? classNames, string suffix)
        {
            if (string.IsNullOrEmpty(classNames))
            {
                return writer;
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                writer.Class(classNames);
                return writer;
            }

            var span = classNames.AsSpan().Trim();

            while (span.Length > 0)
            {
                var index = span.IndexOf(' ');

                if (index > 0)
                {
                    writer.Class($"{span[..index]}-{suffix}");

                    span = span[index..].Trim();
                }
                else
                {
                    break;
                }
            }

            if (span.Length > 0)
            {
                writer.Class($"{span}-{suffix}");
            }

            return writer;
        }
    }
}
