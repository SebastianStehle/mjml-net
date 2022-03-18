using Mjml.Net.Extensions;
using Mjml.Net.Helpers;

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator

namespace Mjml.Net.Components.Body
{
    public partial class ColumnComponent : Component, IProvidesWidth
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

        public ContainerWidth ContainerWidth;
        public double CurrentWidth;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            ContainerWidth = context.GetContainerWidth();

            var (width, widthString, pixels) = GetParsedWidth();
            var widthInner = GetInnerWidth(pixels);

            renderer.ElementStart("div") // Style div
                .Class(GetColumnClass(width, widthString, context))
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
                RenderGutter(renderer, context, widthInner);
            }
            else
            {
                RenderColumn(renderer, context, widthInner);
            }

            renderer.ElementEnd("div");
        }

        private void RenderColumn(IHtmlRenderer renderer, GlobalContext context, double innerWidth)
        {
            var tableElement = renderer.ElementStart("table") // Style table
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

            renderer.ElementStart("tbody");

            if (innerWidth != ContainerWidth.Value)
            {
                context.Push();
                context.SetContainerWidth(innerWidth);
            }

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    child.Render(renderer, context);
                }
                else
                {
                    renderer.ElementStart("tr");
                    renderer.ElementStart("td")
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

                    renderer.ElementEnd("td");
                    renderer.ElementEnd("tr");
                }
            }

            if (innerWidth != ContainerWidth.Value)
            {
                context.Pop();
            }

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private void RenderGutter(IHtmlRenderer renderer, GlobalContext context, double width)
        {
            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Attr("width", "100%");

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td") // Style gutter
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

            RenderColumn(renderer, context, width);

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private static string GetColumnClass((double Value, Unit Unit) width, string originalWidth, GlobalContext context)
        {
            string className;

            if (width.Unit == Unit.Percent)
            {
                className = $"mj-column-per-{width.Value}";
            }
            else
            {
                className = $"mj-column-px-{width.Value}";
            }

            context.SetGlobalData(originalWidth, MediaQuery.Width(className, originalWidth));

            return className;
        }

        private ((double Value, Unit Unit), string, double) GetParsedWidth()
        {
            var widthValue = 0d;
            var widthUnit = Unit.Pixels;
            var widthString = string.Empty;
            var pixels = 0d;

            if (Width != null)
            {
                (widthValue, widthUnit) = UnitParser.Parse(Width);

                // No need to interpolate it again.
                widthString = Width;
            }
            else
            {
                widthValue = 100 / Math.Max(1, Parent?.ChildNodes.Count(x => !x.Raw) ?? 1);
                widthUnit = Unit.Percent;
                widthString = $"{widthValue}%";
            }

            if (widthUnit != Unit.Pixels)
            {
                pixels = ContainerWidth.Value * widthValue / 100;
            }
            else
            {
                pixels = widthValue;
            }

            return ((widthValue, widthUnit), widthString, pixels);
        }

        private double GetInnerWidth(double widthInPixels)
        {
            var allPaddings =
                UnitParser.Parse(PaddingTop).Value +
                UnitParser.Parse(PaddingBottom).Value +
                UnitParser.Parse(BorderLeft).Value +
                UnitParser.Parse(BorderRight).Value +
                UnitParser.Parse(InnerBorderLeft).Value +
                UnitParser.Parse(InnerBorderRight).Value;

            return widthInPixels - allPaddings;
        }

        private bool HasGutter()
        {
            if (!string.IsNullOrEmpty(Padding) ||
                !string.IsNullOrEmpty(PaddingBottom) ||
                !string.IsNullOrEmpty(PaddingLeft) ||
                !string.IsNullOrEmpty(PaddingRight) ||
                !string.IsNullOrEmpty(PaddingTop))
            {
                return true;
            }

            return false;
        }

        public double GetWidthAsPixel()
        {
            return CurrentWidth;
        }
    }
}
