namespace Mjml.Net.Components.Body
{
    public sealed class SpacerComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-spacer";

        public override AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["border"] = AttributeTypes.String,
                ["border-bottom"] = AttributeTypes.String,
                ["border-left"] = AttributeTypes.String,
                ["border-right"] = AttributeTypes.String,
                ["border-top"] = AttributeTypes.String,
                ["container-background-color"] = AttributeTypes.Color,
                ["height"] = AttributeTypes.PixelsOrPercent,
                ["padding"] = AttributeTypes.PixelsOrPercent,
                ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
                ["padding-left"] = AttributeTypes.PixelsOrPercent,
                ["padding-right"] = AttributeTypes.PixelsOrPercent,
                ["padding-top"] = AttributeTypes.PixelsOrPercent
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
