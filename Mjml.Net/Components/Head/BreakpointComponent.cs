using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class BreakpointComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-breakpoint";

        [Bind("width")]
        public string Width;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Width != null)
            {
                context.SetGlobalData("default", new Breakpoint(Width));
            }
        }
    }
}
