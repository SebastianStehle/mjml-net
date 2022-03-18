using System.Globalization;
using Mjml.Net.Components;

namespace Mjml.Net.Extensions
{
    public static class NodeExtensions
    {
        public static ContainerWidth GetContainerWidth(this GlobalContext context)
        {
            return context.Get("containerWidth") as ContainerWidth ?? ContainerWidth.Default;
        }

        public static void SetContainerWidth(this GlobalContext context, double value)
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
