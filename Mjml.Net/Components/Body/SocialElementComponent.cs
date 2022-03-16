namespace Mjml.Net.Components.Body
{
    public partial struct SocialElementProps
    {
        [Bind("align", BindType.Align)]
        public string Align = "left";

        [Bind("background-color", BindType.Color)]
        public string? BackgroundColor;

        [Bind("border-radius", BindType.Pixels)]
        public string BorderRadius = "3px";

        [Bind("color", BindType.Color)]
        public string Color = "#000";

        [Bind("font-family", BindType.String)]
        public string FontFamily = "Ubuntu, Helvetica, Arial, sans-serif";

        [Bind("font-size", BindType.Pixels)]
        public string FontSize = "13px";

        [Bind("font-style", BindType.String)]
        public string? FontStyle;

        [Bind("font-weight", BindType.String)]
        public string? FontWeight;

        [Bind("href", BindType.String)]
        public string? Href;

        [Bind("icon-height", BindType.PixelsOrPercent)]
        public string? IconHeight;

        [Bind("icon-size", BindType.PixelsOrPercent)]
        public string? IconSize;
    }
}
