namespace Mjml.Net.Components.Body
{
    public sealed class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            RenderChildren(renderer);
        }

        private static void RenderChildren(IHtmlRenderer renderer)
        {
            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    child.Render();
                }
            });
        }
    }
}
