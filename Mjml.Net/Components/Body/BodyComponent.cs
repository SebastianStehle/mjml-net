namespace Mjml.Net.Components.Body
{
    public sealed class HeadComponent : IComponent
    {
        public string ComponentName => "mj-body";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.BufferStart();
            renderer.RenderChildren();
            renderer.SetContext("body", renderer.BufferFlush());
        }
    }
}
