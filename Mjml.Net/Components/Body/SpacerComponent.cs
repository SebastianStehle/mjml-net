namespace Mjml.Net.Components.Body
{
    public partial class SpacerComponent : BodyComponentBase
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-column",
            "mj-hero"
        };

        public override AllowedParents? AllowedParents => Parents;

        public override string ComponentName => "mj-spacer";

        [Bind("border")]
        public string? Border;

        [Bind("border-bottom")]
        public string? BorderBottom;

        [Bind("border-left")]
        public string? BorderLeft;

        [Bind("border-right")]
        public string? BorderRight;

        [Bind("border-top")]
        public string? BorderTop;

        [Bind("container-background-color", BindType.Color)]
        public string? ContainerBackgroundColor;

        [Bind("height", BindType.Pixels)]
        public string Height = "20px";

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

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.StartElement("div")
                .Style("height", Height)
                .Style("line-height", Height);

            renderer.Content("&#8202;");

            renderer.EndElement("div");
        }
    }
}
