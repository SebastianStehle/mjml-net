namespace Mjml.Net.Components.Body
{
    public sealed class SectionComponent : IComponent
    {
        public string ComponentName => "mj-section";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            RenderChildren(renderer, node);
        }

        private static void RenderChildren(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren(new ChildOptions<string>
            {
                Context = "100",
                Renderer = RenderChild
            });
        }

        private static void RenderChild(string context, IChildRenderer child, IHtmlRenderer renderer, INode node)
        {
            renderer.StartElement("div")
                .Attr("width", context)
                .Done();

            child.Render();

            renderer.EndElement("link");
        }
    }
}
