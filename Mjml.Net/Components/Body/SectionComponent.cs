using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public sealed class ColumnComponent : IComponent
    {
        public string ComponentName => "mj-column";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            // Set the breakpoint if not set before.
            renderer.SetGlobalData("default", Breakpoint.Default, true);
        }
    }
}
