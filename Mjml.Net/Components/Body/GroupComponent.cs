using Mjml.Net.Extensions;
using Mjml.Net.Helpers;

#pragma warning disable IDE0059 // Unnecessary assignment of a value

namespace Mjml.Net.Components.Body
{
    public partial class GroupComponent : Component
    {
        public override string ComponentName => "mj-group";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("css-class")]
        public string? CssClass;

        [Bind("direction", BindType.Direction)]
        public string Direction = "ltr";

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign;

        [Bind("width", BindType.PixelsOrPercent)]
        public string? Width;

        public (double Value, Unit Unit, string WidthString, double InnerWidth) CurrentWidth;

        public override void Measure(int parentWidth, int numSiblings, int numNonRawSiblings)
        {
            var widthValue = 0d;
            var widthUnit = Unit.Pixels;
            var widthString = string.Empty;

            if (Width != null)
            {
                (widthValue, widthUnit) = UnitParser.Parse(Width);

                widthString = Width;
            }
            else
            {
                widthValue = 100d / Math.Max(1, numNonRawSiblings);
                widthUnit = Unit.Percent;
                widthString = $"{widthValue}%";
            }

            if (widthUnit != Unit.Pixels)
            {
                ActualWidth = (int)(parentWidth * widthValue / 100);
            }
            else
            {
                ActualWidth = (int)widthValue;
            }

            CurrentWidth = (widthValue, widthUnit, widthString, ActualWidth);

            MeasureChildren(ActualWidth);
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("div") // Style div
                .Class(GetColumnClass(context))
                .Class("mj-outlook-group-fix")
                .Class(CssClass)
                .Style("background-color", BackgroundColor)
                .Style("direction", Direction)
                .Style("display", "inline-block")
                .Style("font-size", "0")
                .Style("line-height", "0")
                .Style("text-align", "left")
                .Style("vertical-align", VerticalAlign)
                .Style("width", "100%");

            renderer.Content("<!--[if mso | IE]>");

            renderer.StartElement("table")
                .Attr("bgcolor", BackgroundColor == "none" ? null : BackgroundColor)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");

            renderer.StartElement("tr");

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    renderer.Content("<![endif]-->");
                    child.Render(renderer, context);
                    renderer.Content("<!--[if mso | IE]>");
                }
                else
                {
                    renderer.StartElement("td")
                        .Style("align", child.Node.GetAttribute("align"))
                        .Style("vertical-align", child.Node.GetAttribute("vertical-align"))
                        .Style("width", $"{child.ActualWidth}px");

                    renderer.Content("<![endif]-->");

                    child.Render(renderer, context);

                    renderer.Content("<!--[if mso | IE]>");
                    renderer.EndElement("td");
                }
            }

            renderer.EndElement("tr");
            renderer.EndElement("table");

            renderer.Content("<![endif]-->");

            renderer.EndElement("div");
        }

        private string GetColumnClass(GlobalContext context)
        {
            string className;

            if (CurrentWidth.Unit == Unit.Percent)
            {
                className = $"mj-column-per-{(int)CurrentWidth.Value}";
            }
            else
            {
                className = $"mj-column-px-{(int)CurrentWidth.Value}";
            }

            context.SetGlobalData(className, MediaQuery.Width(className, CurrentWidth.WidthString));

            return className;
        }

        public override string? GetInheritingAttribute(string name)
        {
            switch (name)
            {
                case "mobile-width":
                    return "mobile-width";
            }

            return null;
        }
    }
}
