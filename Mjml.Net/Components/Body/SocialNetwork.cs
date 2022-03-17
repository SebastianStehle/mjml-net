#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it

namespace Mjml.Net.Components.Body
{
    internal sealed record SocialNetwork(string? ShareUrl, string? BackgroundUrl, string? ImageUrl)
    {
        public static readonly SocialNetwork Default = new SocialNetwork(null, null, null);

        private const string ImageBaseUrl = "https://www.mailjet.com/images/theme/v1/icons/ico-social/";

        public static readonly Dictionary<string, SocialNetwork> Defaults = new Dictionary<string, SocialNetwork>
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

        static SocialNetwork()
        {
            foreach (var (key, value) in Defaults.ToList())
            {
                Defaults[$"{key}-noshare"] = value with { ShareUrl = "[[URL]]" };
            }
        }
    }
}
