using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public sealed class ColumnComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-column";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            // Set the breakpoint if not set before.
            renderer.SetGlobalData("default", Breakpoint.Default, true);
        }
    }
}
