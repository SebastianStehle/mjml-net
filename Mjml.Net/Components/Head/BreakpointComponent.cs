using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct BreakpointProperties
    {
        [Bind("width")]
        public string Width;
    }

    public sealed class BreakpointComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-breakpoint";

        public override bool SelfClosed => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var args = new BreakpointProperties(node);

            // Just in case that validation is disabled.
            if (args.Width != null)
            {
                renderer.SetGlobalData("default", new Breakpoint(args.Width));
            }
        }
    }
}
