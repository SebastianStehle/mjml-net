using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public sealed class StyleComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-style";

        public bool NeedsContent => true;

        public AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["inline"] = AttributeType.String
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var style = node.GetContent();

            // Just in case that validation is disabled.
            if (style != null)
            {
                // Allow multiple styles.
                renderer.SetGlobalData(Guid.NewGuid().ToString(), new Style(style));
            }
        }
    }
}
