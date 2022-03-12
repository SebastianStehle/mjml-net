namespace ConsoleApp22.Components
{
    internal class RootComponent : IComponent
    {
        public string ComponentName => "mjml";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren();
        }
    }
}
