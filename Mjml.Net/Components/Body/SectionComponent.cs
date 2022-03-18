using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("background-position", BindType.String)]
        public string BackgroundPosition = "top center";

        [Bind("background-position-x", BindType.String)]
        public string? BackgroundPositionX;

        [Bind("background-position-y", BindType.String)]
        public string? BackgroundPositionY;

        [Bind("background-repeat", BindType.String)]
        public string BackgroundRepeat = "repeat";

        [Bind("background-size", BindType.String)]
        public string BackgroundSize = "auto";

        [Bind("background-url", BindType.String)]
        public string? BackgroundUrl;

        [Bind("border", BindType.String)]
        public string? Border;

        [Bind("border-bottom", BindType.String)]
        public string? BorderBottom;

        [Bind("border-left", BindType.String)]
        public string? BorderLeft;

        [Bind("border-radius", BindType.String)]
        public string? BorderRadius;

        [Bind("border-right", BindType.String)]
        public string? BorderRight;

        [Bind("border-top", BindType.String)]
        public string? BorderTop;

        [Bind("direction", BindType.String)]
        public string Direction = "ltr";

        [Bind("full-width", BindType.String)]
        public string? FullWidth;

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "20px 0";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("text-align", BindType.Align)]
        public string TextAlign = "center";

        [Bind("text-padding", BindType.PixelsOrPercent)]
        public string TextPadding = "4px 4px 4px 0";

        [Bind("css-class")]
        public string? CssClass;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (IsFullWidth())
            {
                RenderFullWidth(renderer, ref context);
            }
            else
            {
                RenderSimple(renderer, ref context);
            }
        }

        private void RenderFullWidth(IHtmlRenderer renderer, ref GlobalContext context)
        {
            var hasBackground = HasBackground();

            var tableElement = renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("background", BackgroundUrl)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Class(CssClass)
                .Style("width", "100%")
                .Style("border-radius", BorderRadius);

            if (IsFullWidth())
            {
                if (hasBackground)
                {
                    tableElement
                        .Style("background", GetBackground())
                        .Style("background-color", BackgroundColor)
                        .Style("background-position", BackgroundPosition)
                        .Style("background-repeat", BackgroundRepeat)
                        .Style("background-size", BackgroundSize);
                }
                else
                {
                    tableElement
                        .Style("background", BackgroundColor)
                        .Style("background-color", BackgroundColor);
                }
            }

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td");

            if (hasBackground)
            {
                RenderSectionWithBackground(renderer, ref context);
            }
            else
            {
                RenderSectionStart(renderer, ref context);
                RenderSection(renderer, ref context);
                RenderSectionEnd(renderer);
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }

        private void RenderSimple(IHtmlRenderer renderer, ref GlobalContext context)
        {
            RenderSectionStart(renderer, ref context);

            if (HasBackground())
            {
                RenderSectionWithBackground(renderer, ref context);
            }
            else
            {
                RenderSection(renderer, ref context);
            }

            RenderSectionEnd(renderer);
        }

        private void RenderSectionStart(IHtmlRenderer renderer, ref GlobalContext context)
        {
            var containerWidth = context.GetContainerWidth();

            renderer.StartConditionalTag();
            renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", IsFullWidth() ? "100%" : containerWidth.String)
                .Attr("bgcolor", BackgroundColor)
                .Attr("class", CssClass?.SuffixCssClasses("outlook"))
                .Style("width", IsFullWidth() ? "100%" : containerWidth.StringWithUnit);

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("line-height", "0px")
                .Style("font-size", "0px")
                .Style("mso-line-height-rule", "exactly");
            renderer.EndConditionalTag();
        }

        private void RenderSection(IHtmlRenderer renderer, ref GlobalContext context)
        {
            var containerWidth = context.GetContainerWidth();
            var isFullWidth = IsFullWidth();
            var hasBackground = HasBackground();
            var background = hasBackground ? GetBackground() : null;

            var divElement = renderer.ElementStart("div")
                           .Attr("class", isFullWidth ? null : CssClass)
                           .Style("margin", "0px auto")
                           .Style("border-radius", BorderRadius)
                           .Style("max-width", containerWidth.StringWithUnit);

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    divElement
                        .Style("background", background)
                        .Style("background-color", BackgroundColor)
                        .Style("background-position", BackgroundPosition)
                        .Style("background-repeat", BackgroundRepeat)
                        .Style("background-size", BackgroundSize);
                }
                else
                {
                    divElement
                        .Style("background", BackgroundColor)
                        .Style("background-color", BackgroundColor);
                }
            }

            if (hasBackground)
            {
                renderer.ElementStart("div")
                    .Style("line-height", "0")
                    .Style("font-size", "0");
            }

            var tableElement = renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("background", isFullWidth ? null : BackgroundUrl)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("border-radius", BorderRadius);

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    tableElement
                        .Style("background", background)
                        .Style("background-color", BackgroundColor)
                        .Style("background-position", BackgroundPosition)
                        .Style("background-repeat", BackgroundRepeat)
                        .Style("background-size", BackgroundSize);
                }
                else
                {
                    tableElement
                        .Style("background", BackgroundColor)
                        .Style("background-color", BackgroundColor);
                }
            }

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("border", Border)
                .Style("border-bottom", BorderBottom)
                .Style("border-left", BorderLeft)
                .Style("border-right", BorderRight)
                .Style("border-top", BorderTop)
                .Style("direction", Direction)
                .Style("font-size", "0px")
                .Style("padding", Padding)
                .Style("padding-bottom", PaddingBottom)
                .Style("padding-left", PaddingLeft)
                .Style("padding-right", PaddingRight)
                .Style("padding-top", PaddingTop)
                .Style("text-align", TextAlign);

            renderer.StartConditionalTag();
            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");
            renderer.EndConditionalTag();

            RenderChildren(renderer, ref context);

            renderer.StartConditionalTag();
            renderer.ElementEnd("table");
            renderer.EndConditionalTag();

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");

            if (hasBackground)
            {
                renderer.ElementEnd("div");
            }

            renderer.ElementEnd("div");
        }

        private static void RenderSectionEnd(IHtmlRenderer renderer)
        {
            renderer.StartConditionalTag();

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.EndConditionalTag();
        }

        private void RenderSectionWithBackground(IHtmlRenderer renderer, ref GlobalContext context)
        {
            // TODO: https://github.com/mjmlio/mjml/blob/a5812ac1ad7cdf7ef9ae71fcf5808c49ba8ac5cb/packages/mjml-section/src/index.js#L265-L407

            var isFullwidth = IsFullWidth();
            var containerWidth = context.GetContainerWidth();

            var (x, y) = ParseBackgroundPosition();
            var (xPercent, yPercent) = GetBackgroundPositionAsPercentage(ref x, ref y);

            var (xOrigin, xPosition) = GetOriginBasedForAxis("x", ref xPercent, ref yPercent);
            var (yOrigin, yPosition) = GetOriginBasedForAxis("y", ref xPercent, ref yPercent);

            var isBackgroundSizeAuto = BackgroundSize.Equals("auto", StringComparison.OrdinalIgnoreCase);
            var isBackgroundSizeCover = BackgroundSize.Equals("cover", StringComparison.OrdinalIgnoreCase);
            var isBackgroundSizeContain = BackgroundSize.Equals("contain", StringComparison.OrdinalIgnoreCase);
            var isBackgroundRepeatNoRepeat = BackgroundRepeat.Equals("no-repeat", StringComparison.OrdinalIgnoreCase);

            string? vmlSize = null;
            string? vmlAspect = null;
            string? vmlType = isBackgroundRepeatNoRepeat ? "frame" : "tile";

            if (isBackgroundSizeCover || isBackgroundSizeContain)
            {
                vmlSize = "1,1";
                vmlAspect = isBackgroundSizeCover ? "atleast" : "atmost";
            }
            else if (!isBackgroundSizeAuto)
            {
                var positions = BackgroundSize.Split(' ');

                if (positions.Length == 1)
                {
                    vmlSize = positions[0];
                    vmlAspect = "atmost";
                }
                else
                {
                    vmlSize = string.Join(',', positions);
                }
            }

            if (isBackgroundSizeAuto)
            {
                vmlType = "tile";
                xOrigin = "0.5";
                xPosition = "0.5";
                yOrigin = "0";
                yPosition = "0";
            }

            renderer.StartConditionalTag();
            var rectElement = renderer.ElementStart("v:rect")
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("fill", "true")
                .Attr("stroke", "false");

            if (isFullwidth)
            {
                rectElement.Style("mso-width-percent", "1000");
            }
            else
            {
                rectElement.Style("width", containerWidth.StringWithUnit);
            }

            renderer.ElementStart("v:fill", true)
                .Attr("origin", $"{xOrigin}, {yOrigin}")
                .Attr("position", $"{xPosition}, {yPosition}")
                .Attr("src", BackgroundUrl)
                .Attr("color", BackgroundColor)
                .Attr("type", vmlType)
                .Attr("size", vmlSize)
                .Attr("aspect", vmlAspect);

            renderer.ElementStart("v:textbox")
                .Attr("inset", "0,0,0,0")
                .Style("mso-fit-shape-to-text", "true");
            renderer.EndConditionalTag();

            RenderSection(renderer, ref context);

            renderer.StartConditionalTag();
            renderer.ElementEnd("v:textbox");
            renderer.ElementEnd("v:rect");
            renderer.EndConditionalTag();
        }

        private void RenderChildren(IHtmlRenderer renderer, ref GlobalContext context)
        {
            renderer.StartConditionalTag();
            renderer.ElementStart("tr");
            renderer.EndConditionalTag();

            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    child.Render(renderer, context);
                }
                else
                {
                    // ONCE MJ-COLUMN is complete this will be updated
                    renderer.StartConditionalTag();
                    renderer.ElementStart("td")
                        .Attr("align", child.Node.GetAttribute("align"))
                        .Attr("class", child.Node.GetAttribute("css-class")?.SuffixCssClasses("outlook"))
                        .Style("vertical-align", child.Node.GetAttribute("vertical-align"))
                        .Style("width", child.Node.GetAttribute("width")); //  getWidthAsPixel
                    renderer.EndConditionalTag();

                    child.Render(renderer, context);

                    renderer.StartConditionalTag();
                    renderer.ElementEnd("td");
                    renderer.EndConditionalTag();
                }
            }

            renderer.StartConditionalTag();
            renderer.ElementEnd("tr");
            renderer.EndConditionalTag();
        }

        private string? GetBackground()
        {
            if (!HasBackground())
            {
                return BackgroundColor;
            }

            return $"{BackgroundColor} url({BackgroundUrl}) {GetBackgroundPositionString()} / {BackgroundSize} {BackgroundRepeat}";
        }

        private string? GetBackgroundPositionString()
        {
            (string parsedX, string parsedY) = ParseBackgroundPosition();

            var x = string.IsNullOrEmpty(BackgroundPositionX) ? parsedX : BackgroundPositionX;
            var y = string.IsNullOrEmpty(BackgroundPositionY) ? parsedY : BackgroundPositionY;

            return $"{x} {y}";
        }

        private (string x, string y) ParseBackgroundPosition()
        {
            var positions = BackgroundPosition.Split(' ');

            bool IsTopOrBottom(ref string axis)
            {
                if (axis.Equals("top", StringComparison.OrdinalIgnoreCase) || axis.Equals("bottom", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }

            switch (positions.Length)
            {
                case 1:
                    if (IsTopOrBottom(ref positions[0]))
                    {
                        return ("center", positions[0]);
                    }

                    return (positions[0], "center");

                case 2:
                    if (IsTopOrBottom(ref positions[0]) || (positions[0].Equals("center", StringComparison.OrdinalIgnoreCase) && IsTopOrBottom(ref positions[1])))
                    {
                        return (positions[1], positions[0]);
                    }

                    return (positions[0], positions[1]);

                default:
                    return ("center", "top");
            }
        }

        private (string xPercent, string yPercent) GetBackgroundPositionAsPercentage(ref string backgroundPositionX, ref string backgroundPositionY)
        {
            var xPercent = backgroundPositionX;
            var yPercent = backgroundPositionY;

            switch (backgroundPositionX.ToLowerInvariant())
            {
                case "left":
                    xPercent = "0%";
                    break;

                case "center":
                    xPercent = "50%";
                    break;

                case "right":
                    xPercent = "100%";
                    break;

                default:
                    if (!backgroundPositionX.Contains("%", StringComparison.OrdinalIgnoreCase))
                    {
                        xPercent = "50%";
                    }
                    break;
            }

            switch (backgroundPositionY.ToLowerInvariant())
            {
                case "top":
                    yPercent = "0%";
                    break;

                case "center":
                    yPercent = "50%";
                    break;

                case "bottom":
                    yPercent = "100%";
                    break;

                default:
                    if (!backgroundPositionY.Contains("%", StringComparison.OrdinalIgnoreCase))
                    {
                        xPercent = "0%";
                    }
                    break;
            }

            return (xPercent, yPercent);
        }

        private (string origin, string position) GetOriginBasedForAxis(string axis, ref string positionX, ref string positionY)
        {
            var isX = axis.Equals("x", StringComparison.OrdinalIgnoreCase);
            var isBackgroundRepeat = BackgroundRepeat.Equals("repeat", StringComparison.OrdinalIgnoreCase);

            var position = isX ? positionX : positionY;
            var origin = isX ? positionX : positionY;

            if (position.Contains("%", StringComparison.OrdinalIgnoreCase))
            {
                var positionUnit = UnitParser.Parse(position);
                var positionUnitDouble = positionUnit.Value / 100.0;

                if (isBackgroundRepeat)
                {
                    var temp = positionUnitDouble.ToInvariantString();
                    position = temp;
                    origin = temp;
                }
                else
                {
                    var temp = -50.0 + (positionUnitDouble * 100 / 100);
                    position = $"{temp}";
                    origin = $"{temp}";
                }
            }
            else if (isBackgroundRepeat)
            {
                var temp = isX ? "0.5" : "0";
                position = temp;
                origin = temp;
            }
            else
            {
                var temp = isX ? "0" : "-0.5";
                position = temp;
                origin = temp;
            }

            return (origin, position);
        }

        private bool HasBackground()
        {
            return BackgroundUrl != null;
        }

        private bool IsFullWidth()
        {
            if (string.IsNullOrEmpty(FullWidth))
            {
                return false;
            }

            return FullWidth.Equals("full-width", StringComparison.OrdinalIgnoreCase);
        }
    }
}
