namespace Mjml.Net.Components.Body
{
    public sealed class SpacerComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-spacer";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["border"] = AttributeType.String,
                ["border-bottom"] = AttributeType.String,
                ["border-left"] = AttributeType.String,
                ["border-right"] = AttributeType.String,
                ["border-top"] = AttributeType.String,
                ["container-background-color"] = AttributeType.Color,
                ["height"] = AttributeType.PixelsOrPercent,
                ["padding"] = AttributeType.PixelsOrPercent,
                ["padding-bottom"] = AttributeType.PixelsOrPercent,
                ["padding-left"] = AttributeType.PixelsOrPercent,
                ["padding-right"] = AttributeType.PixelsOrPercent,
                ["padding-top"] = AttributeType.PixelsOrPercent,
            };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["height"] = "20px"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var height = node.GetAttribute("height");

            renderer.ElementStart("div")
                .Style("height", height)
                .Style("line-height", height);

            renderer.Content("&#8202;");

            renderer.ElementEnd("div");
        }
    }
}
