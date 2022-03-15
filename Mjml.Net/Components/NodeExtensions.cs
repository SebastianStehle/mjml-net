using System.Globalization;

namespace Mjml.Net.Components
{
    public static class NodeExtensions
    {
        public static ContainerWidth GetContainerWidth(this IContext renderer)
        {
            return renderer.Get("containerWidth") as ContainerWidth ?? ContainerWidth.Default;
        }

        public static void SetContainerWidth(this IContext context, double value)
        {
            context.Set("containerWidth", new ContainerWidth(value, $"{value}", $"{value}px"));
        }

        public static string ToInvariantString(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
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
