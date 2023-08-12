using System.Text;
using Mjml.Net;

namespace Tests.Internal;

public partial class TestComponent : Component
{
    public override string ComponentName => "mjml-test";

    [Bind("head")]
    public string? Head;

    [Bind("body")]
    public string? Body;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        RenderChildren(renderer, context);

        if (Head != "false")
        {
            RenderHead(renderer, context);
        }

        if (Body != "false")
        {
            RenderBody(renderer, context);
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
