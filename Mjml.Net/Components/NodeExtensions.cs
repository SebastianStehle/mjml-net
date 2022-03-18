using System.Globalization;

namespace Mjml.Net.Components
{
    public static class NodeExtensions
    {
        public static ContainerWidth GetContainerWidth(this IContext context)
        {
            return context.Get("containerWidth") as ContainerWidth ?? ContainerWidth.Default;
        }

        public static void SetContainerWidth(this IContext context, double value)
        {
            context.Set("containerWidth", ContainerWidth.Create(value));
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
