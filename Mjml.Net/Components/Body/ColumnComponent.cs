using Mjml.Net.Extensions;
using Mjml.Net.Helpers;
using Mjml.Net.Types;

#pragma warning disable IDE0059 // Unnecessary assignment of a value

namespace Mjml.Net.Components.Body
{
    public partial class ColumnComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-group",
            "mj-section"
        };

        public override AllowedParents? AllowedParents => Parents;

        public override string ComponentName => "mj-column";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border")]
        public string? Border;

        [Bind("border-bottom")]
        public string? BorderBottom;

        [Bind("border-left")]
        public string? BorderLeft;

        [Bind("border-radius", BindType.FourPixelsOrPercent)]
        public string? BorderRadius;

        [Bind("border-right")]
        public string? BorderRight;

        [Bind("border-top")]
        public string? BorderTop;

        [Bind("direction", BindType.Direction)]
        public string Direction = "ltr";

        [Bind("inner-background-color", BindType.Color)]
        public string? InnerBackgroundColor;

        [Bind("inner-border")]
        public string? InnerBorder;

        [Bind("inner-border-bottom")]
        public string? InnerBorderBottom;

        [Bind("inner-border-left")]
        public string? InnerBorderLeft;

        [Bind("inner-border-radius", BindType.FourPixelsOrPercent)]
        public string? InnerBorderRadius;

        [Bind("inner-border-right")]
        public string? InnerBorderRight;

        [Bind("inner-border-top")]
        public string? InnerBorderTop;

        [Bind("mobile-width")]
        public string? MobileWidth;

        [Bind("padding", BindType.FourPixelsOrPercent)]
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

        public override void Measure(double parentWidth, int numSiblings, int numNonRawSiblings)
        {
            var widthValue = 0d;
            var widthUnit = Unit.Pixels;
            var widthString = string.Empty;

            if (Width != null)
            {
                (widthValue, widthUnit) = UnitParser.Parse(Width, Unit.Pixels);

                widthString = Width;
            }
            else
            {
                widthValue = 100d / Math.Max(1, numNonRawSiblings);
                widthUnit = Unit.Percent;
                widthString = FormattableString.Invariant($"{widthValue}%");
            }

            if (widthUnit != Unit.Pixels)
            {
                ActualWidth = parentWidth * widthValue / 100;
            }
            else
            {
                ActualWidth = widthValue;
            }

            var allPaddings =
                UnitParser.Parse(PaddingLeft).Value +
                UnitParser.Parse(PaddingRight).Value +
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
                .Style("width", GetWidth()); // Overriden by mj-column-per-*

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

        private string GetWidth()
        {
            if (MobileWidth != "mobile-width")
            {
                return "100%";
            }

            return CurrentWidth.WidthString;
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
                        .Attr("align", child.GetAttribute("align"))
                        .Attr("class", child.GetAttribute("css-class"))
                        .Attr("vertical-align", child.GetAttribute("vertical-align"))
                        .Style("background", child.GetAttribute("container-background-color"))
                        .Style("font-size", "0px")
                        .Style("padding", child.GetAttribute("padding"))
                        .Style("padding-bottom", child.GetAttribute("padding-bottom"))
                        .Style("padding-left", child.GetAttribute("padding-left"))
                        .Style("padding-right", child.GetAttribute("padding-right"))
                        .Style("padding-top", child.GetAttribute("padding-top"))
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

            var widthValue = CurrentWidth.Value.ToInvariantString().Replace('.', '-');

            if (CurrentWidth.Unit == Unit.Percent)
            {
                className = FormattableString.Invariant($"mj-column-per-{widthValue}");
            }
            else
            {
                className = FormattableString.Invariant($"mj-column-px-{widthValue}");
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
