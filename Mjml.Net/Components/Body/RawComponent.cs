namespace Mjml.Net.Components.Body
{
    public sealed class RawComponent : BodyComponentBase
    {
        public override string ComponentName => "mj-raw";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren(new ChildOptions
            {
                RawXML = true
            });
        }
    }
}
