namespace Mjml.Net.Components
{
    public partial class FallbackProps
    {
        [Bind("noop")]
        public string? Noop;
    }

    internal class FallbackComponent : Component<FallbackProps>
    {
        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
        }
    }
}
