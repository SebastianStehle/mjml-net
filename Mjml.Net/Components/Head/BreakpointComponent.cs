using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class BreakpointProps
    {
        [Bind("width")]
        public string Width;
    }

    public sealed class BreakpointComponent : HeadComponentBase<BreakpointProps>
    {
        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Props.Width != null)
            {
                context.SetGlobalData("default", new Breakpoint(Props.Width));
            }
        }
    }
}
