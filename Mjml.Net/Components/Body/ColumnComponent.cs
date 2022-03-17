using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial class ColumnComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-col";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Set the breakpoint if not set before.
            context.SetGlobalData("default", Breakpoint.Default, true);
        }
    }
}
