namespace Mjml.Net.Components.Body
{
    public partial class BodyProps
    {
        [Bind("noop")]
        public string? Noop;
    }

    public sealed class BodyComponent : Component<BodyProps>
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mjml"
        };

        public override AllowedParents? AllowedAsChild => Parents;

        public override string Name => "mj-body";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.BufferStart();

            RenderChildren(renderer, context);

            context.SetGlobalData("body", renderer.BufferFlush());
        }
    }
}
