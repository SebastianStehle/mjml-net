namespace Mjml.Net.Components.Body
{
    public partial struct SocialElementProps
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

    public sealed class SocialElementComponent : BodyComponentBase<SocialComponentProps>
    {
        private const string ImageBaseUrl = "https://www.mailjet.com/images/theme/v1/icons/ico-social/";

        private readonly Dictionary<string, SocialNetwork> socialNetworks = new Dictionary<string, SocialNetwork>
        {
            ["facebook"] = new SocialNetwork(
                "https://www.facebook.com/sharer/sharer.php?u=[[URL]]",
                "#3b5998",
                $"{ImageBaseUrl}facebook.png"),
            ["twitter"] = new SocialNetwork(
                "https://twitter.com/intent/tweet?url=[[URL]]",
                "#55acee",
                $"{ImageBaseUrl}twitter.png"),
            ["google"] = new SocialNetwork(
                "https://plus.google.com/share?url=[[URL]]",
                "#dc4e41",
                $"{ImageBaseUrl}google-plus.png"),
            ["pinterest"] = new SocialNetwork(
                "https://pinterest.com/pin/create/button/?url=[[URL]]&media=&description=",
                "#bd081c",
                $"{ImageBaseUrl}pinterest.png"),
            ["linkedin"] = new SocialNetwork(
                "https://www.linkedin.com/shareArticle?mini=true&url=[[URL]]&title=&summary=&source=",
                "#0077b5",
                $"{ImageBaseUrl}linkedin.png"),
            ["instagram"] = new SocialNetwork(
                null,
                "#3f729b",
                $"{ImageBaseUrl}instagram.png"),
            ["web"] = new SocialNetwork(
                null,
                "#4BADE9",
                $"{ImageBaseUrl}web.png"),
            ["snapchat"] = new SocialNetwork(
                null,
                "#FFFA54",
                $"{ImageBaseUrl}snapchat.png"),
            ["youtube"] = new SocialNetwork(
                null,
                "#EB3323",
                $"{ImageBaseUrl}youtube.png"),
            ["tumblr"] = new SocialNetwork(
                "https://www.tumblr.com/widgets/share/tool?canonicalUrl=[[URL]]",
                "#344356",
                $"{ImageBaseUrl}tumblr.png"),
            ["github"] = new SocialNetwork(
                null,
                "#000000",
                $"{ImageBaseUrl}github.png"),
            ["xing"] = new SocialNetwork(
                "https://www.xing.com/app/user?op=share&url=[[URL]]",
                "#296366",
                $"{ImageBaseUrl}xing.png"),
            ["vimeo"] = new SocialNetwork(
                null,
                "#53B4E7",
                $"{ImageBaseUrl}vimeo.png"),
            ["medium"] = new SocialNetwork(
                null,
                "#000000",
                $"{ImageBaseUrl}medium.png"),
            ["soundcloud"] = new SocialNetwork(
                null,
                "#EF7F31",
                $"{ImageBaseUrl}soundcloud.png"),
            ["dribbble"] = new SocialNetwork(
                null,
                "#D95988",
                $"{ImageBaseUrl}dribbble.png")
        };

        public SocialElementComponent()
        {
            foreach (var (key, value) in socialNetworks.ToList())
            {
                socialNetworks[$"{key}-noshare"] = value with { ShareUrl = "[[URL]]" };
            }
        }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it
        private sealed record SocialNetwork(string? ShareUrl, string? BackgroundUrl, string? ImageUrl)
        {
            public static readonly SocialNetwork Default = new SocialNetwork(null, null, null);
        }
#pragma warning restore RECS0082 // Parameter has the same name as a member and hides it
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

        public override string ComponentName => "mj-social-element";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new SocialElementProps(node);

            var (socialNetwork, href) = GetSocialAttributes(ref props);

            renderer.ElementStart("tr")
                .Class(props.Class);

            renderer.ElementStart("td") // Style td
                .Style("padding", props.Padding)
                .Style("vertical-align", props.VerticalAlign);

            renderer.ElementStart("table") // Style table
                .Attr("border", "0")
                .Attr("cellpadding", "0")
                .Attr("cellspacing", "0")
                .Attr("role", "presentation")
                .Style("background", socialNetwork.BackgroundUrl)
                .Style("border-radius", props.BorderRadius)
                .Style("width", props.IconSize);

            renderer.ElementStart("tbody");
            renderer.ElementStart("tr");

            renderer.ElementStart("td") // Style icon
                .Style("font-size", "0")
                .Style("height", props.IconHeight ?? props.IconSize)
                .Style("padding", props.IconPadding)
                .Style("vertical-align", "middle")
                .Style("width", props.IconSize);

            if (href != null)
            {
                renderer.ElementStart("a")
                    .Attr("href", href)
                    .Attr("rel", props.Rel)
                    .Attr("target", props.Target);
            }

            renderer.ElementStart("img") // Style img
                .Attr("alt", props.Alt)
                .Attr("height", UnitParser.Parse(props.IconHeight ?? props.IconSize).Value.ToInvariantString())
                .Attr("sizes", props.Sizes)
                .Attr("src", socialNetwork.ImageUrl)
                .Attr("srcset", props.Srcset)
                .Attr("title", props.Title)
                .Attr("width", UnitParser.Parse(props.IconSize).Value.ToInvariantString())
                .Style("border-radius", props.BorderRadius)
                .Style("display", "block");

            if (props.Href != null)
            {
                renderer.ElementEnd("a");
            }

            renderer.ElementEnd("td");
            renderer.ElementEnd("tr");
            renderer.ElementEnd("tbody");
            renderer.ElementEnd("table");
            renderer.ElementEnd("td");

            if (props.Text != null)
            {
                renderer.ElementStart("td") // Style tdText
                    .Style("padding", props.TextPadding)
                    .Style("vertical-align", "middle");

                if (props.Href != null)
                {
                    renderer.ElementStart("a") // Style text
                        .Attr("href", href)
                        .Attr("rel", props.Rel)
                        .Attr("target", props.Target)
                        .Style("color", props.Color)
                        .Style("font-family", props.FontFamily)
                        .Style("font-size", props.FontSize)
                        .Style("font-style", props.FontStyle)
                        .Style("font-weigh", props.FontWeight)
                        .Style("line-height", props.LineHeight)
                        .Style("text-decoration", props.TextDecoration);
                }
                else
                {
                    renderer.ElementStart("span") // Style text
                        .Style("color", props.Color)
                        .Style("font-family", props.FontFamily)
                        .Style("font-size", props.FontSize)
                        .Style("font-style", props.FontStyle)
                        .Style("font-weigh", props.FontWeight)
                        .Style("line-height", props.LineHeight)
                        .Style("text-decoration", props.TextDecoration);
                }

                renderer.Content(props.Text);

                if (props.Href != null)
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

        private (SocialNetwork, string?) GetSocialAttributes(ref SocialElementProps props)
        {
            var socialNetwork = SocialNetwork.Default;

            if (props.Name != null && socialNetworks.TryGetValue(props.Name, out var network))
            {
                socialNetwork = network;
            }

            var href = props.Href;

            if (href != null && socialNetwork.ShareUrl != null)
            {
                href = socialNetwork.ShareUrl.Replace("[[URL]]", href, StringComparison.Ordinal);
            }

            if (props.BackgroundColor != null)
            {
                socialNetwork = socialNetwork with { BackgroundUrl = props.BackgroundColor };
            }

            if (props.Src != null)
            {
                socialNetwork = socialNetwork with { ImageUrl = props.Src };
            }

            return (socialNetwork, href);
        }
    }
}
