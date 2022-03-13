namespace Mjml.Net.Components.Body
{
    public sealed class SectionComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-section";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            RenderChildren(renderer, node);
        }

        private static void RenderChildren(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren(new ChildOptions
            {
                Renderer = child =>
                {
                    renderer.ElementStart("div")
                        .Attr("width", node.GetAttribute("width"));

                    child.Render();

                    renderer.ElementEnd("link");
                }
            });
        }
    }
}
