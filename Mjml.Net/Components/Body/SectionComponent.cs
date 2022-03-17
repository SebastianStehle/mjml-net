using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial struct SectionProps
    {
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
    }

    public sealed class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new SectionProps(node);

            if (IsFullWidth(node))
            {
                RenderFullWidth(renderer, node, ref props);
            }
            else
            {
                RenderSimple(renderer, node, ref props);
            }
        }

        private void RenderFullWidth(IHtmlRenderer renderer, INode node, ref SectionProps props)
        {
            throw new NotImplementedException();
        }

        private static void RenderSimple(IHtmlRenderer renderer, INode node, ref SectionProps props)
        {
            RenderSectionStart(renderer, node, ref props);

            if (HasBackground(node))
            {
                RenderSectionWithBackground(renderer, node, ref props);
            }
            else
            {
                RenderSection(renderer, node, ref props);
            }

            RenderSectionEnd(renderer, node);
        }

        private static void RenderSectionStart(IHtmlRenderer renderer, INode node, ref SectionProps props)
        {
            renderer.StartConditionalTag();
            renderer.ElementStart("table")
                .Attr("align", "center")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("width", IsFullWidth(node) ? "100%" : renderer.GetContainerWidth().String)
                .Attr("bgcolor", props.BackgroundColor)
                .Attr("class", node.GetAttribute("css-class")?.SuffixCssClasses("outlook"))
                .Style("width", IsFullWidth(node) ? "100%" : renderer.GetContainerWidth().StringWithUnit);

            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("line-height", "0px")
                .Style("font-size", "0px")
                .Style("mso-line-height-rule", "exactly");
            renderer.EndConditionalTag();
        }

        private static void RenderSection(IHtmlRenderer renderer, INode node, ref SectionProps props)
        {
            var isFullWidth = IsFullWidth(node);
            var hasBackground = HasBackground(node);

            var containerOuterwidth = renderer.GetContainerWidth().StringWithUnit;

            var divElement = renderer.ElementStart("div")
                           .Attr("class", isFullWidth ? null : node.GetAttribute("css-class"))
                           .Style("margin", "0px auto")
                           .Style("border-radius", props.BorderRadius)
                           .Style("max-width", containerOuterwidth);

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    divElement
                        .Style("background", GetBackground(node, ref props))
                        .Style("background-color", props.BackgroundColor)
                        .Style("background-position", props.BackgroundPosition)
                        .Style("background-repeat", props.BackgroundRepeat)
                        .Style("background-size", props.BackgroundSize);
                }
                else
                {
                    divElement
                        .Style("background", props.BackgroundColor)
                        .Style("background-color", props.BackgroundColor);
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
                .Attr("background", isFullWidth ? null : props.BackgroundUrl)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("width", "100%")
                .Style("border-radius", props.BorderRadius);

            if (!isFullWidth)
            {
                if (hasBackground)
                {
                    tableElement
                        .Style("background", GetBackground(node, ref props))
                        .Style("background-color", props.BackgroundColor)
                        .Style("background-position", props.BackgroundPosition)
                        .Style("background-repeat", props.BackgroundRepeat)
                        .Style("background-size", props.BackgroundSize);
                }
                else
                {
                    tableElement
                        .Style("background", props.BackgroundColor)
                        .Style("background-color", props.BackgroundColor);
                }
            }

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");
            renderer.ElementStart("td")
                .Style("border", props.Border)
                .Style("border-bottom", props.BorderBottom)
                .Style("border-left", props.BorderLeft)
                .Style("border-right", props.BorderRight)
                .Style("border-top", props.BorderTop)
                .Style("direction", props.Direction)
                .Style("font-size", "0px")
                .Style("padding", props.Padding)
                .Style("padding-bottom", props.PaddingBottom)
                .Style("padding-left", props.PaddingLeft)
                .Style("padding-right", props.PaddingRight)
                .Style("padding-top", props.PaddingTop)
                .Style("text-align", props.TextAlign);

            renderer.StartConditionalTag();
            renderer.ElementStart("table")
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");
            renderer.EndConditionalTag();

            RenderChildren(renderer);

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

        private static string? GetBackground(INode node, ref SectionProps props)
        {
            throw new NotImplementedException();
        }

        private static void RenderSectionEnd(IHtmlRenderer renderer, INode node)
        {
            renderer.StartConditionalTag();

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");

            renderer.EndConditionalTag();
        }

        private static void RenderSectionWithBackground(IHtmlRenderer renderer, INode node, ref SectionProps props)
        {
            throw new NotImplementedException();
        }

        private static void RenderChildren(IHtmlRenderer renderer)
        {
            renderer.StartConditionalTag();
            renderer.ElementStart("tr");
            renderer.EndConditionalTag();

            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    if (child.Node.Component.Raw)
                    {
                        child.Render();
                    }
                    else
                    {
                        if (child.Node.Component.ComponentName.Equals("mj-column", StringComparison.InvariantCultureIgnoreCase))
                        {
                            renderer.StartConditionalTag();
                            renderer.ElementStart("td")
                               .Attr("align", child.Node.GetAttribute("align"))
                               .Attr("class", child.Node.GetAttribute("css-class")?.SuffixCssClasses("outlook"))
                               .Style("vertical-align", child.Node.GetAttribute("vertical-align"))
                               .Style("width", child.Node.GetAttribute("width")); //  getWidthAsPixel
                            renderer.EndConditionalTag();

                            child.Render();
                        }
                    }
                }
            });

            renderer.StartConditionalTag();
            renderer.ElementEnd("tr");
            renderer.EndConditionalTag();
        }

        private static bool HasBackground(INode node)
        {
            return node.GetAttribute("background-url") != null;
        }

        private static bool IsFullWidth(INode node)
        {
            return node.GetAttribute("full-width") == "full-width";
        }
    }
}
