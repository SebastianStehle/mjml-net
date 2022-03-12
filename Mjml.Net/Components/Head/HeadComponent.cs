namespace Mjml.Net.Components.Head
{
    public sealed class HeadComponent : IComponent
    {
        public string ComponentName => "mj-head";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren();
        }
    }
}
