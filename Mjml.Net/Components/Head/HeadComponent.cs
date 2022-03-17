namespace Mjml.Net.Components.Head
{
    public partial class HeadProps
    {
        [Bind("noop")]
        public string? Noop;
    }

    public sealed class HeadComponent : Component<HeadProps>
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mjml"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.BufferStart();

            RenderChildren(renderer, context);

            context.SetGlobalData("head", renderer.BufferFlush());
        }
    }
}
