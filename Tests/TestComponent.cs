using System.Text;
using Mjml.Net;

namespace Tests
{
    public sealed class TestComponent : Component
    {
        public override string ComponentName => "mjml-test";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderChildren(renderer, context);

            if (Node.GetAttribute("body") != "false")
            {
                RenderBody(renderer, context);
            }

            if (Node.GetAttribute("head") != "false")
            {
                RenderHead(renderer, context);
            }
        }

        private static void RenderHead(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.RenderHelpers(HelperTarget.HeadStart);

            if (context.GlobalData.TryGetValue((typeof(StringBuilder), "head"), out var head) && head is StringBuilder sb)
            {
                renderer.Plain(sb);
            }

            renderer.RenderHelpers(HelperTarget.HeadEnd);
        }

        private static void RenderBody(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.RenderHelpers(HelperTarget.BodyStart);

            if (context.GlobalData.TryGetValue((typeof(StringBuilder), "body"), out var body) && body is StringBuilder sb)
            {
                renderer.Plain(sb);
            }

            renderer.RenderHelpers(HelperTarget.BodyEnd);
        }
    }
}
