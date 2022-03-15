using Mjml.Net;

namespace Tests
{
    public sealed class TestComponent : IComponent
    {
        public string ComponentName => "mjml-test";

        public AllowedParents? AllowedParents { get; } = null;

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren();

            RenderHead(renderer);
            RenderBody(renderer);
        }

        private static void RenderHead(IHtmlRenderer renderer)
        {
            renderer.RenderHelpers(HelperTarget.HeadStart);

            if (renderer.GlobalData.TryGetValue((typeof(string), "head"), out var head))
            {
                renderer.Plain(head.ToString());
            }

            renderer.RenderHelpers(HelperTarget.HeadEnd);
        }

        private static void RenderBody(IHtmlRenderer renderer)
        {
            renderer.RenderHelpers(HelperTarget.BodyStart);

            if (renderer.GlobalData.TryGetValue((typeof(string), "body"), out var body))
            {
                renderer.Plain(body.ToString());
            }

            renderer.RenderHelpers(HelperTarget.BodyEnd);
        }
    }
}
