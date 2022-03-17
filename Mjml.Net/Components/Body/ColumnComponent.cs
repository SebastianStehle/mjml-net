using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public partial class ColumnProps
    {
        [Bind("none")]
        public string? None;
    }

    public sealed class ColumnComponent : BodyComponentBase<ColumnProps>
    {
        public override string Name => "mj-col";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Set the breakpoint if not set before.
            context.SetGlobalData("default", Breakpoint.Default, true);
        }
    }
}
