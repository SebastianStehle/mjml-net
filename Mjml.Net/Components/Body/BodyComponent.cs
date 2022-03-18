using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial class BodyComponent : Component
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mjml"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override string ComponentName => "mj-body";

        [Bind("css-class")]
        public string? CssClass;

        [Bind("width")]
        public string Width = "600px";

        [Bind("background-color")]
        public string? BackgroundColor;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.BufferStart();

            renderer.StartElement("div")
                .Attr("background-color", BackgroundColor)
                .Class(CssClass)
                .Style("background-color", BackgroundColor);

            context.Push();
            context.SetContainerWidth(UnitParser.Parse(Width).Value);

            RenderChildren(renderer, context);

            context.Pop();

            renderer.EndElement("div");

            context.SetGlobalData("body", renderer.BufferFlush());
        }
    }
}
