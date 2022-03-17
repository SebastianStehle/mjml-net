﻿namespace Mjml.Net.Components.Body
{
    public partial struct SocialComponentProps
    {
        [Bind("align", BindType.Align)]
        public string Align = "center";

        [Bind("border-radius")]
        public string BorderRadius = "3px";

        [Bind("color")]
        public string Color = "#333333";

        [Bind("container-background-color", BindType.Color)]
        public string ContainerBackgroundColor;

        [Bind("font-family")]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle;

        [Bind("font-weight")]
        public string? FontWeight;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string? IconHeight;

        [Bind("icon-padding", BindType.FourPixelsOrPercent)]
        public string? IconPadding;

        [Bind("icon-size", BindType.PixelsOrPercent)]
        public string IconSize = "20px";

        [Bind("inner-padding", BindType.FourPixelsOrPercent)]
        public string? InnerPadding;

        [Bind("line-height", BindType.PixelsOrPercent)]
        public string LineHeight = "22px";

        [Bind("mode", BindType.SocialMode)]
        public string Mode = "horizontal";

        [Bind("padding", BindType.FourPixelsOrPercent)]
        public string Padding = "10px 25px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("table-layout", BindType.SocialTableLayout)]
        public string? TableLayout;

        [Bind("text-decoration")]
        public string TextDecoration = "none";

        [Bind("text-padding", BindType.FourPixelsOrPercent)]
        public string? TextPadding;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string? VerticalAlign;
    }

    public sealed class SocialComponent : BodyComponentBase<SocialComponentProps>
    {
        public override string ComponentName => "mj-social"; 

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new SocialComponentProps(node);

            if (props.Mode == "horizontal")
            {
                RenderHorizontal(renderer, node, ref props);
            }
            else
            {
                RenderVertical(renderer);
            }
        }

        private static void RenderHorizontal(IHtmlRenderer renderer, INode node, ref SocialComponentProps props)
        {
            renderer.Content("<!--[if mso | IE]>");

            renderer.ElementStart("table")
                .Attr("align", props.Align)
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation");

            renderer.ElementStart("tr");

            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    if (child.Node.Component.Raw)
                    {
                        renderer.Content("<![endif]-->");
                        child.Render();
                        renderer.Content("<!--[if mso | IE]>");
                    }
                    else
                    {
                        renderer.ElementStart("td");
                        renderer.Content("<![endif]-->");

                        renderer.ElementStart("table")
                            .Attr("align", child.Node.GetAttribute("align"))
                            .Attr("border", "0")
                            .Attr("cellpadding", "0")
                            .Attr("cellspacing", "0")
                            .Attr("role", "presentation")
                            .Style("display", "inline-table")
                            .Style("float", "none");

                        renderer.ElementStart("tbody");

                        child.Render();

                        renderer.ElementEnd("tbody");
                        renderer.ElementEnd("table");

                        renderer.Content("<!--[if mso | IE]>");
                        renderer.ElementEnd("td");
                    }
                },
                ChildResolver = CreateResolver(props)
            });

            renderer.ElementEnd("tr");
            renderer.ElementEnd("table");
            renderer.Content("<![endif]-->");
        }

        private static Func<string, string?> CreateResolver(SocialComponentProps props)
        {
            return name =>
            {
                switch (name)
                {
                    case "border-radius":
                        return props.BorderRadius;
                    case "color":
                        return props.Color;
                    case "font-family":
                        return props.FontFamily;
                    case "font-size":
                        return props.FontSize;
                    case "font-style":
                        return props.FontStyle;
                    case "icon-height":
                        return props.IconHeight;
                    case "icon-padding":
                        return props.IconPadding;
                    case "icon-size":
                        return props.IconSize;
                    case "line-height":
                        return props.IconHeight;
                    case "text-padding":
                        return props.TextPadding;
                    case "text-decoration":
                        return props.TextDecoration;
                    default:
                        return null;
                }
            };
        }

        private static void RenderVertical(IHtmlRenderer renderer)
        {
            renderer.ElementStart("table") // Table-vertical
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("margin", "0px");

            renderer.ElementStart("tbody");

            renderer.RenderChildren();

            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
        }
    }
}
