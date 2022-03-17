using Mjml.Net;
using Mjml.Net.Components;

namespace Tests
{
    public sealed class TestComponent : Component<FallbackProps>
    {
        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderChildren(renderer, context);

            RenderHead(renderer, context);
            RenderBody(renderer, context);
        }

        private static void RenderHead(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.RenderHelpers(HelperTarget.HeadStart);

            if (context.GlobalData.TryGetValue((typeof(string), "head"), out var head))
            {
                renderer.Plain(head.ToString());
            }

            renderer.RenderHelpers(HelperTarget.HeadEnd);
        }

        private static void RenderBody(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.RenderHelpers(HelperTarget.BodyStart);

            if (context.GlobalData.TryGetValue((typeof(string), "body"), out var body))
            {
                renderer.Plain(body.ToString());
            }

            renderer.RenderHelpers(HelperTarget.BodyEnd);
        }
    }
}
