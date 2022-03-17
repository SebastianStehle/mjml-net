namespace Mjml.Net.Components.Head
{
    public sealed class HeadComponent : IComponent
    {
        public string ComponentName => "mj-head";

        public AllowedParents? AllowedAsChild { get; } =
            new AllowedParents
            {
                "mjml"
            };

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.BufferStart();
            renderer.RenderChildren();
            renderer.SetGlobalData("head", renderer.BufferFlush());
        }
    }
}
