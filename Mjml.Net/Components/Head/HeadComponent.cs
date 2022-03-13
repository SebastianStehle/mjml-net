namespace Mjml.Net.Components.Head
{
    public sealed class HeadComponent : IComponent
    {
        public string ComponentName => "mj-head";

        public AllowedParents? AllowedParents { get; } =
            new AllowedParents
            {
                "mjml"
            };

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.BufferStart();
            renderer.RenderChildren();
            renderer.SetContext("head", renderer.BufferFlush());
        }
    }
}
