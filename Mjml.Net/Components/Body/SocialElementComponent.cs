namespace Mjml.Net.Components.Body
{
    public partial class SocialElementProps
    {
        [Bind("align", BindType.Align)]
        public string Align = "left";

        [Bind("alt")]
        public string? Alt;

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border-radius", BindType.Pixels)]
        public string BorderRadius = "3px";

        [Bind("color", BindType.Color)]
        public string Color = "#000";

        [Bind("css-class")]
        public string? Class;

        [Bind("font-family")]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style")]
        public string? FontStyle;

        [Bind("font-weight")]
        public string? FontWeight;

        [Bind("href")]
        public string? Href;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string? IconHeight;

        [Bind("icon-padding", BindType.PixelsOrPercent)]
        public string? IconPadding;

        [Bind("icon-size", BindType.PixelsOrPercent)]
        public string? IconSize;

        [Bind("line-height", BindType.PixelsOrPercentOrNone)]
        public string LineHeight = "22px";

        [Bind("name")]
        public string? Name;

        [Bind("padding", BindType.PixelsOrPercent)]
        public string Padding = "4px";

        [Bind("padding-bottom", BindType.PixelsOrPercent)]
        public string? PaddingBottom;

        [Bind("padding-left", BindType.PixelsOrPercent)]
        public string? PaddingLeft;

        [Bind("padding-right", BindType.PixelsOrPercent)]
        public string? PaddingRight;

        [Bind("padding-top", BindType.PixelsOrPercent)]
        public string? PaddingTop;

        [Bind("rel")]
        public string? Rel;

        [Bind("sizes")]
        public string? Sizes;

        [Bind("src")]
        public string? Src;

        [Bind("srcset")]
        public string? Srcset;

        [Bind("target")]
        public string Target = "_blank";

        [Bind("text-decoration")]
        public string TextDecoration = "none";

        [Bind("text-padding", BindType.PixelsOrPercent)]
        public string TextPadding = "4px 4px 4px 0";

        [Bind("title")]
        public string? Title;

        [Bind("vertical-align", BindType.VerticalAlign)]
        public string VerticalAlign = "middle";

        [BindText]
        public string? Text;
    }

    public sealed class SocialElementComponent : BodyComponentBase<SocialElementProps>
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mg-social"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override string Name => "mj-social-element";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var (socialNetwork, href) = GetSocialAttributes();

            renderer.ElementStart("tr")
                .Class(Props.Class);

            renderer.ElementStart("td") // Style td
                .Style("padding", Props.Padding)
                .Style("vertical-align", Props.VerticalAlign);

            renderer.ElementStart("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("background", socialNetwork.BackgroundUrl)
                .Style("border-radius", Props.BorderRadius)
                .Style("width", Props.IconSize);

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");

            renderer.ElementStart("td") // Style icon
                .Style("font-size", "0")
                .Style("height", Props.IconHeight ?? Props.IconSize)
                .Style("padding", Props.IconPadding)
                .Style("vertical-align", "middle")
                .Style("width", Props.IconSize);

            if (href != null)
            {
                renderer.ElementStart("a")
                    .Attr("href", href)
                    .Attr("rel", Props.Rel)
                    .Attr("target", Props.Target);
            }

            renderer.ElementStart("img") // Style img
                .Attr("alt", Props.Alt)
                .Attr("height", UnitParser.Parse(Props.IconHeight ?? Props.IconSize).Value.ToInvariantString())
                .Attr("sizes", Props.Sizes)
                .Attr("src", socialNetwork.ImageUrl)
                .Attr("srcset", Props.Srcset)
                .Attr("title", Props.Title)
                .Attr("width", UnitParser.Parse(Props.IconSize).Value.ToInvariantString())
                .Style("border-radius", Props.BorderRadius)
                .Style("display", "block");

            if (Props.Href != null)
            {
                renderer.ElementEnd("a");
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("td");

            if (Props.Text != null)
            {
                renderer.ElementStart("td") // Style tdText
                    .Style("padding", Props.TextPadding)
                    .Style("vertical-align", "middle");

                if (Props.Href != null)
                {
                    renderer.ElementStart("a") // Style text
                        .Attr("href", href)
                        .Attr("rel", Props.Rel)
                        .Attr("target", Props.Target)
                        .Style("color", Props.Color)
                        .Style("font-family", Props.FontFamily)
                        .Style("font-size", Props.FontSize)
                        .Style("font-style", Props.FontStyle)
                        .Style("font-weigh", Props.FontWeight)
                        .Style("line-height", Props.LineHeight)
                        .Style("text-decoration", Props.TextDecoration);
                }
                else
                {
                    renderer.ElementStart("span") // Style text
                        .Style("color", Props.Color)
                        .Style("font-family", Props.FontFamily)
                        .Style("font-size", Props.FontSize)
                        .Style("font-style", Props.FontStyle)
                        .Style("font-weigh", Props.FontWeight)
                        .Style("line-height", Props.LineHeight)
                        .Style("text-decoration", Props.TextDecoration);
                }

                renderer.Content(Props.Text);

                if (Props.Href != null)
                {
                    renderer.ElementEnd("a");
                }
                else
                {
                    renderer.ElementEnd("span");
                }
            }

            renderer.ElementEnd("tr");
        }

        private (SocialNetwork, string?) GetSocialAttributes()
        {
            var socialNetwork = SocialNetwork.Default;

            if (Props.Name != null && SocialNetwork.Defaults.TryGetValue(Props.Name, out var network))
            {
                socialNetwork = network;
            }

            var href = Props.Href;

            if (href != null && socialNetwork.ShareUrl != null)
            {
                href = socialNetwork.ShareUrl.Replace("[[URL]]", href, StringComparison.Ordinal);
            }

            if (Props.BackgroundColor != null)
            {
                socialNetwork = socialNetwork with { BackgroundUrl = Props.BackgroundColor };
            }

            if (Props.Src != null)
            {
                socialNetwork = socialNetwork with { ImageUrl = Props.Src };
            }

            return (socialNetwork, href);
        }
    }
}
