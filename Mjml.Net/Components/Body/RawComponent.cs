namespace Mjml.Net.Components.Body
{
    public partial class RawComponent : BodyComponentBase
    {
        public override ComponentType Type => ComponentType.Raw;

        public override string ComponentName => "mj-raw";

        public override bool Raw => true;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderRaw(renderer);
        }
    }
}
