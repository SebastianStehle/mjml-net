namespace ConsoleApp22.Components.Head
{
    internal class HeadComponent : IComponent
    {
        public string ComponentName => "mj-head";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.SetContext("head", new HeadContext());

            renderer.RenderChildren();
        }
    }
}
