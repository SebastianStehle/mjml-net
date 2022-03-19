using Mjml.Net.Helpers;

#pragma warning disable IDE0059 // Unnecessary assignment of a value

namespace Mjml.Net.Components.Body
{
    public partial class ColumnComponent : Component
    {
        public override string ComponentName => "mj-column";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border", BindType.String)]
        public string? Border;

        [Bind("border-bottom", BindType.String)]
        public string? BorderBottom;

        [Bind("border-left", BindType.String)]
        public string? BorderLeft;

        [Bind("border-radius", BindType.PixelsOrPercent)]
        public string? BorderRadius;

        [Bind("border-right", BindType.String)]
        public string? BorderRight;

        [Bind("border-top", BindType.String)]
        public string? BorderTop;

        [Bind("css-class")]
        public string? CssClass;

        [Bind("direction", BindType.Direction)]
        public string Direction = "ltr";

        [Bind("inner-background-color", BindType.Color)]
        public string? InnerBackgroundColor;

        [Bind("inner-border", BindType.String)]
        public string? InnerBorder;

        [Bind("inner-border-bottom", BindType.String)]
        public string? InnerBorderBottom;

        [Bind("inner-border-left", BindType.String)]
        public string? InnerBorderLeft;

        [Bind("inner-border-radius", BindType.PixelsOrPercent)]
        public string? InnerBorderRadius;

        [Bind("inner-border-right", BindType.String)]
        public string? InnerBorderRight;

        [Bind("inner-border-top", BindType.String)]
        public string? InnerBorderTop;

        [Bind("padding", BindType.PixelsOrPercent)]
        public string? Padding;

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string VerticalAlign = "top";

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

            var allPaddings =
                UnitParser.Parse(PaddingTop).Value +
                UnitParser.Parse(PaddingBottom).Value +
                UnitParser.Parse(BorderLeft).Value +
                UnitParser.Parse(BorderRight).Value +
                UnitParser.Parse(InnerBorderLeft).Value +
                UnitParser.Parse(InnerBorderRight).Value;

            var innerWidth = ActualWidth - (int)allPaddings;

            CurrentWidth = (widthValue, widthUnit, widthString, ActualWidth - allPaddings);

            MeasureChildren(innerWidth);
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("div") // Style div
                .Class(GetColumnClass(context))
                .Class("mj-outlook-group-fix")
                .Class(CssClass)
                .Style("direction", Direction)
                .Style("display", "inline-block")
                .Style("font-size", "0px")
                .Style("text-align", "left")
                .Style("vertical-align", VerticalAlign)
                .Style("width", "100%"); // Overriden by mj-column-per-*

            if (HasGutter())
            {
                RenderGutter(renderer, context);
            }
            else
            {
                RenderColumn(renderer, context);
            }

            renderer.EndElement("div");
        }

        private void RenderColumn(IHtmlRenderer renderer, GlobalContext context)
        {
            var tableElement = renderer.StartElement("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", "100%");

            if (HasGutter())
            {
                tableElement // Style `table` with gutter
                    .Style("background-color", InnerBackgroundColor)
                    .Style("border", InnerBorder)
                    .Style("border-bottom", InnerBorderBottom)
                    .Style("border-left", InnerBorderLeft)
                    .Style("border-radius", InnerBorderRadius)
                    .Style("border-right", InnerBorderRight)
                    .Style("border-top", InnerBorderTop);
            }
            else
            {
                tableElement // Style `table`
                    .Style("background-color", BackgroundColor)
                    .Style("border", Border)
                    .Style("border-bottom", BorderBottom)
                    .Style("border-left", BorderLeft)
                    .Style("border-radius", BorderRadius)
                    .Style("border-right", BorderRight)
                    .Style("border-top", BorderTop)
                    .Style("vertical-align", VerticalAlign);
            }

            renderer.StartElement("tbody");

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    child.Render(renderer, context);
                }
                else
                {
                    renderer.StartElement("tr");
                    renderer.StartElement("td")
                        .Attr("align", child.Node.GetAttribute("align"))
                        .Attr("class", child.Node.GetAttribute("css-class"))
                        .Attr("vertical-align", child.Node.GetAttribute("vertical-align"))
                        .Style("background", child.Node.GetAttribute("container-background-color"))
                        .Style("font-size", "0px")
                        .Style("padding", child.Node.GetAttribute("padding"))
                        .Style("padding-bottom", child.Node.GetAttribute("padding-bottom"))
                        .Style("padding-left", child.Node.GetAttribute("padding-left"))
                        .Style("padding-right", child.Node.GetAttribute("padding-right"))
                        .Style("padding-top", child.Node.GetAttribute("padding-top"))
                        .Style("word-break", "break-word");

                    child.Render(renderer, context);

                    renderer.EndElement("td");
                    renderer.EndElement("tr");
                }
            }

            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        private void RenderGutter(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", "100%");

            renderer.StartElement("tbody");
            renderer.StartElement("tr");
            renderer.StartElement("td") // Style gutter
                .Style("background-color", BackgroundColor)
                .Style("border", Border)
                .Style("border-bottom", BorderBottom)
                .Style("border-left", BorderLeft)
                .Style("border-radius", BorderRadius)
                .Style("border-right", BorderRight)
                .Style("border-top", BorderTop)
                .Style("padding", Padding)
                .Style("padding-bottom", PaddingBottom)
                .Style("padding-left", PaddingLeft)
                .Style("padding-right", PaddingRight)
                .Style("padding-top", PaddingTop)
                .Style("vertical-align", VerticalAlign);

            RenderColumn(renderer, context);

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
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

        private bool HasGutter()
        {
            if (!string.IsNullOrEmpty(PaddingBottom) ||
                !string.IsNullOrEmpty(PaddingLeft) ||
                !string.IsNullOrEmpty(PaddingRight) ||
                !string.IsNullOrEmpty(PaddingTop))
            {
                return true;
            }

            return false;
        }
    }
}
