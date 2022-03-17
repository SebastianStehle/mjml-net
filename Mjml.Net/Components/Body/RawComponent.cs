namespace Mjml.Net.Components.Body
{
    public partial class RawProps
    {
        [Bind("none")]
        public string? None;
    }

    public sealed class RawComponent : BodyComponentBase<RawProps>
    {
        public override ComponentType Type => ComponentType.Raw;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderRaw(renderer);
        }
    }
}
