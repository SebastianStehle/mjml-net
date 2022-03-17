namespace Mjml.Net.Components.Body
{
    public partial class BodyComponent : Component
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mjml"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override string ComponentName => "mj-body";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.BufferStart();

            RenderChildren(renderer, context);

            context.SetGlobalData("body", renderer.BufferFlush());
        }
    }
}
