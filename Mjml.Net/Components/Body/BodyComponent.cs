using Mjml.Net.Helpers;

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

        [Bind("background-color")]
        public string? BackgroundColor;

        [Bind("width", BindType.Pixels)]
        public string Width = "600px";

        public override void Measure(int parentWidth, int numSiblings, int numNonRawSiblings)
        {
            ActualWidth = (int)UnitParser.Parse(Width).Value;

            MeasureChildren(ActualWidth);
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (BackgroundColor != null)
            {
                context.SetGlobalData("default", new Background(BackgroundColor));
            }

            renderer.BufferStart();

            renderer.StartElement("div")
                .Class(CssClass)
                .Style("background-color", BackgroundColor);

            RenderChildren(renderer, context);

            renderer.EndElement("div");

            context.SetGlobalData("body", renderer.BufferFlush());
        }
    }
}
