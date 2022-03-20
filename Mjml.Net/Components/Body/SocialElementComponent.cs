namespace Mjml.Net.Components.Body
{
    public partial class SocialElementComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mg-social"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override ContentType ContentType => ContentType.Text;

        public override string ComponentName => "mj-social-element";

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
        public string? CssClass;

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

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            var (socialNetwork, href) = GetSocialAttributes();

            renderer.StartElement("tr")
                .Class(CssClass);

            renderer.StartElement("td") // Style td
                .Style("padding", Padding)
                .Style("vertical-align", VerticalAlign);

            renderer.StartElement("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("background", socialNetwork.BackgroundUrl)
                .Style("border-radius", BorderRadius)
                .Style("width", IconSize);

            renderer.StartElement("tbody");
            renderer.StartElement("tr");

            renderer.StartElement("td") // Style icon
                .Style("font-size", "0")
                .Style("height", IconHeight ?? IconSize)
                .Style("padding", IconPadding)
                .Style("vertical-align", "middle")
                .Style("width", IconSize);

            if (href != null)
            {
                renderer.StartElement("a")
                    .Attr("href", href)
                    .Attr("rel", Rel)
                    .Attr("target", Target);
            }

            renderer.StartElement("img") // Style img
                .Attr("alt", Alt)
                .Attr("height", $"{UnitParser.Parse(IconHeight ?? IconSize).Value}")
                .Attr("sizes", Sizes)
                .Attr("src", socialNetwork.ImageUrl)
                .Attr("srcset", Srcset)
                .Attr("title", Title)
                .Attr("width", $"{UnitParser.Parse(IconSize).Value}")
                .Style("border-radius", BorderRadius)
                .Style("display", "block");

            if (Href != null)
            {
                renderer.EndElement("a");
            }

            renderer.EndElement("td");
            renderer.EndElement("tr");
            renderer.EndElement("tbody");
            renderer.EndElement("table");
            renderer.EndElement("td");

            if (Text != null)
            {
                renderer.StartElement("td") // Style tdText
                    .Style("padding", TextPadding)
                    .Style("vertical-align", "middle");

                if (Href != null)
                {
                    renderer.StartElement("a") // Style text
                        .Attr("href", href)
                        .Attr("rel", Rel)
                        .Attr("target", Target)
                        .Style("color", Color)
                        .Style("font-family", FontFamily)
                        .Style("font-size", FontSize)
                        .Style("font-style", FontStyle)
                        .Style("font-weigh", FontWeight)
                        .Style("line-height", LineHeight)
                        .Style("text-decoration", TextDecoration);
                }
                else
                {
                    renderer.StartElement("span") // Style text
                        .Style("color", Color)
                        .Style("font-family", FontFamily)
                        .Style("font-size", FontSize)
                        .Style("font-style", FontStyle)
                        .Style("font-weigh", FontWeight)
                        .Style("line-height", LineHeight)
                        .Style("text-decoration", TextDecoration);
                }

                renderer.Content(Text);

                if (Href != null)
                {
                    renderer.EndElement("a");
                }
                else
                {
                    renderer.EndElement("span");
                }
            }

            renderer.EndElement("tr");
        }

        private (SocialNetwork, string?) GetSocialAttributes()
        {
            var socialNetwork = SocialNetwork.Default;

            if (Name != null && SocialNetwork.Defaults.TryGetValue(Name, out var network))
            {
                socialNetwork = network;
            }

            var href = Href;

            if (href != null && socialNetwork.ShareUrl != null)
            {
                href = socialNetwork.ShareUrl.Replace("[[URL]]", href, StringComparison.Ordinal);
            }

            if (BackgroundColor != null)
            {
                socialNetwork = socialNetwork with { BackgroundUrl = BackgroundColor };
            }

            if (Src != null)
            {
                socialNetwork = socialNetwork with { ImageUrl = Src };
            }

            return (socialNetwork, href);
        }
    }
}
