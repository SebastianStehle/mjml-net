using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public sealed class BreakpointComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-breakpoint";

        public bool SelfClosed => true;

        public AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["width"] = AttributeTypes.String
            };

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var width = node.GetAttribute("width");

            // Just in case that validation is disabled.
            if (width != null)
            {
                renderer.SetGlobalData("default", new Breakpoint(width));
            }
        }
    }
}
