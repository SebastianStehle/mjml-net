namespace Mjml.Net.Components.Body
{
    public sealed class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        public override AllowedAttributes? AllowedAttributes { get; } =
          new AllowedAttributes
          {
              ["background-color"] = AttributeTypes.Color,
              ["background-position"] = AttributeTypes.String,
              ["background-position-x"] = AttributeTypes.String,
              ["background-position-y"] = AttributeTypes.String,
              ["background-repeat"] = AttributeTypes.String,
              ["background-size"] = AttributeTypes.String,
              ["background-url"] = AttributeTypes.String,
              ["border"] = AttributeTypes.String,
              ["border-bottom"] = AttributeTypes.String,
              ["border-left"] = AttributeTypes.String,
              ["border-radius"] = AttributeTypes.FourPixelsOrPercent,
              ["border-right"] = AttributeTypes.String,
              ["border-top"] = AttributeTypes.String,
              ["css-class"] = AttributeTypes.String,
              ["direction"] = AttributeTypes.String,
              ["full-width"] = AttributeTypes.String,
              ["padding"] = AttributeTypes.FourPixelsOrPercent,
              ["padding-bottom"] = AttributeTypes.PixelsOrPercent,
              ["padding-left"] = AttributeTypes.PixelsOrPercent,
              ["padding-right"] = AttributeTypes.PixelsOrPercent,
              ["padding-top"] = AttributeTypes.PixelsOrPercent,
              ["text-align"] = AttributeTypes.Align
          };

        public override Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["background-position"] = "top center",
                ["background-repeat"] = "repeat",
                ["background-size"] = "auto",
                ["border"] = "none",
                ["direction"] = "ltr",
                ["padding"] = "20px 0",
                ["text-align"] = "center"
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            RenderChildren(renderer, node);
        }

        private static void RenderChildren(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    renderer.ElementStart("div")
                        .Attr("width", node.GetAttribute("width"));

                    child.Render();

                    renderer.ElementEnd("link");
                }
            });
        }
    }
}
