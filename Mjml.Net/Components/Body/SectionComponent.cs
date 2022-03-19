using Mjml.Net.Extensions;

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator

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

        [Bind("css-class")]
        public string? CssClass;

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

        public ContainerWidth ContainerWidth;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            ContainerWidth = context.GetContainerWidth();

            if (IsFullWidth())
            {
                RenderFullWidth(renderer, context);
            }
            else
            {
                RenderSimple(renderer, context);
            }
        }

        private void RenderFullWidth(IHtmlRenderer renderer, GlobalContext context)
        {
            var hasBackground = HasBackground();

            var tableElement = renderer.StartElement("table")
                .Attr("align", "center")
                .Attr("background", BackgroundUrl)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Class(CssClass)
                .Style("width", "100%")
                .Style("border-radius", BorderRadius);

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

            renderer.StartElement("tbody");
            renderer.StartElement("tr");
            renderer.StartElement("td");

            if (hasBackground)
            {
                RenderSectionWithBackground(renderer, context);
            }
            else
            {
                RenderSectionStart(renderer);
                RenderSection(renderer, context);
                RenderSectionEnd(renderer);
            }

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
        }

        private void RenderSimple(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderSectionStart(renderer);

            if (HasBackground())
            {
                RenderSectionWithBackground(renderer, context);
            }
            else
            {
                RenderSection(renderer, context);
            }

            RenderSectionEnd(renderer);
        }

        private void RenderSectionStart(IHtmlRenderer renderer)
        {
            renderer.Content("<!--[if mso | IE]>");
            renderer.StartElement("table")
                .Attr("align", "center")
                .Attr("bgcolor", BackgroundColor)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("class", CssClass?.SuffixCssClasses("outlook"))
                .Attr("width", IsFullWidth() ? "100%" : ContainerWidth.String)
                .Style("width", IsFullWidth() ? "100%" : ContainerWidth.StringWithUnit);

            renderer.StartElement("tr");
            renderer.StartElement("td")
                .Style("font-size", "0px")
                .Style("line-height", "0px")
                .Style("mso-line-height-rule", "exactly");
            renderer.Content("<![endif]-->");
        }

        private void RenderSection(IHtmlRenderer renderer, GlobalContext context)
        {
            var isFullWidth = IsFullWidth();
            var hasBackground = HasBackground();
            var background = hasBackground ? GetBackground() : null;

            var divElement = renderer.StartElement("div") // Style div
                .Attr("class", isFullWidth ? null : CssClass)
                .Style("border-radius", BorderRadius)
                .Style("margin", "0px auto")
                .Style("max-width", ContainerWidth.StringWithUnit);

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    divElement // Style background
                        .Style("background", background)
                        .Style("background-color", BackgroundColor)
                        .Style("background-position", BackgroundPosition)
                        .Style("background-repeat", BackgroundRepeat)
                        .Style("background-size", BackgroundSize);
                }
                else
                {
                    divElement // Style background
                        .Style("background", BackgroundColor)
                        .Style("background-color", BackgroundColor);
                }
            }

            if (hasBackground)
            {
                renderer.StartElement("div") // Style innerDiv
                    .Style("line-height", "0")
                    .Style("font-size", "0");
            }

            var tableElement = renderer.StartElement("table") // Style table
                .Attr("align", "center")
                .Attr("background", isFullWidth ? null : BackgroundUrl)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("border-radius", BorderRadius)
                .Style("width", "100%");

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    tableElement // Style background
                        .Style("background", background)
                        .Style("background-color", BackgroundColor)
                        .Style("background-position", BackgroundPosition)
                        .Style("background-repeat", BackgroundRepeat)
                        .Style("background-size", BackgroundSize);
                }
                else
                {
                    tableElement // Style background
                        .Style("background", BackgroundColor)
                        .Style("background-color", BackgroundColor);
                }
            }

            renderer.StartElement("tbody");
            renderer.StartElement("tr");
            renderer.StartElement("td") // Style td
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

            renderer.Content("<!--[if mso | IE]>");
            renderer.StartElement("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");

            RenderColumns(renderer, context);

            renderer.EndElement("table");
            renderer.Content("<![endif]-->");

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");

            if (hasBackground)
            {
                renderer.EndElement("div");
            }

            renderer.EndElement("div");
        }

        private static void RenderSectionEnd(IHtmlRenderer renderer)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("table");

            renderer.Content("<![endif]-->");
        }

        private void RenderSectionWithBackground(IHtmlRenderer renderer, GlobalContext context)
        {
            var isFullwidth = IsFullWidth();

            var (x, y) = ParseBackgroundPosition();
            var (xPercent, yPercent) = GetBackgroundPositionAsPercentage(x, y);

            var (xOrigin, xPosition) = GetOriginBasedForAxis(true, xPercent, yPercent);
            var (yOrigin, yPosition) = GetOriginBasedForAxis(false, xPercent, yPercent);

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
                yOrigin = "0";
                xPosition = "0.5";
                yPosition = "0";
            }

            renderer.Content("<!--[if mso | IE]>");
            var rectElement = renderer.StartElement("v:rect")
                .Attr("xmlns:v", "urn:schemas-microsoft-com:vml")
                .Attr("fill", "true")
                .Attr("stroke", "false");

            if (isFullwidth)
            {
                rectElement.Style("mso-width-percent", "1000");
            }
            else
            {
                rectElement.Style("width", ContainerWidth.StringWithUnit);
            }

            renderer.StartElement("v:fill", true)
                .Attr("origin", $"{xOrigin}, {yOrigin}")
                .Attr("position", $"{xPosition}, {yPosition}")
                .Attr("src", BackgroundUrl)
                .Attr("color", BackgroundColor)
                .Attr("type", vmlType)
                .Attr("size", vmlSize)
                .Attr("aspect", vmlAspect);

            renderer.StartElement("v:textbox")
                .Attr("inset", "0,0,0,0")
                .Style("mso-fit-shape-to-text", "true");
            renderer.Content("<![endif]-->");

            RenderSection(renderer, context);

            renderer.Content("<!--[if mso | IE]>");
            renderer.EndElement("v:textbox");
            renderer.EndElement("v:rect");
            renderer.Content("<![endif]-->");
        }

        private void RenderColumns(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("tr");

            var innerWidth =
                ContainerWidth.Value -
                UnitParser.Parse(BorderLeft).Value -
                UnitParser.Parse(BorderRight).Value -
                UnitParser.Parse(PaddingLeft).Value -
                UnitParser.Parse(PaddingRight).Value;

            if (innerWidth != ContainerWidth.Value)
            {
                context.Push();
                context.SetContainerWidth(innerWidth);
            }

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
                        .Attr("align", child.Node.GetAttribute("align"))
                        .Attr("class", child.Node.GetAttribute("css-class")?.SuffixCssClasses("outlook"))
                        .Style("vertical-align", child.Node.GetAttribute("vertical-align"))
                        .Style("width", GetElementWidth(child, context));
                    renderer.Content("<![endif]-->");

                    child.Render(renderer, context);

                    renderer.Content("<!--[if mso | IE]>");
                    renderer.EndElement("td");
                }
            }

            if (innerWidth != ContainerWidth.Value)
            {
                context.Pop();
            }

            renderer.EndElement("tr");
        }

        private static string GetElementWidth(IComponent component, GlobalContext context)
        {
            var width = 0d;

            if (component is IProvidesWidth providesWidth)
            {
                width = providesWidth.GetWidthAsPixel(context);
            }

            return $"{width}px";
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
            var (parsedX, parsedY) = ParseBackgroundPosition();

            var x = string.IsNullOrEmpty(BackgroundPositionX) ? parsedX : BackgroundPositionX;
            var y = string.IsNullOrEmpty(BackgroundPositionY) ? parsedY : BackgroundPositionY;

            return $"{x} {y}";
        }

        private (string x, string y) ParseBackgroundPosition()
        {
            var positions = BackgroundPosition.Split(' ');

            static bool IsTopOrBottom(ref string axis)
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

        private static (string xPercent, string yPercent) GetBackgroundPositionAsPercentage(string backgroundPositionX, string backgroundPositionY)
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
                    if (!backgroundPositionX.Contains('%', StringComparison.OrdinalIgnoreCase))
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
                    if (!backgroundPositionY.Contains('%', StringComparison.OrdinalIgnoreCase))
                    {
                        xPercent = "0%";
                    }

                    break;
            }

            return (xPercent, yPercent);
        }

        private (string Origin, string Position) GetOriginBasedForAxis(bool isXAxis, string positionX, string positionY)
        {
            var position = isXAxis ? positionX : positionY;

            if (position.Contains('%', StringComparison.OrdinalIgnoreCase))
            {
                var positionUnit = UnitParser.Parse(position);
                var positionUnitDouble = positionUnit.Value / 100.0;

                if (BackgroundRepeat == "repeat")
                {
                    var temp = positionUnitDouble.ToInvariantString();

                    return (temp, temp);
                }
                else
                {
                    var temp = $"{(positionUnitDouble * 100 / 100) - 50.0}";

                    return (temp, temp);
                }
            }
            else if (BackgroundRepeat == "repeat")
            {
                var temp = isXAxis ? "0.5" : "0";

                return (temp, temp);
            }
            else
            {
                var temp = isXAxis ? "0" : "-0.5";

                return (temp, temp);
            }
        }

        private bool HasBackground()
        {
            return BackgroundUrl != null;
        }

        private bool IsFullWidth()
        {
            return FullWidth == "full-width";
        }
    }
}
