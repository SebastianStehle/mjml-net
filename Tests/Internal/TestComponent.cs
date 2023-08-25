using Mjml.Net;
using Mjml.Net.Components;

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

        foreach (var (_, value) in context.GlobalData)
        {
            if (value is HeadBuffer head && head.Buffer != null)
            {
                // Already formatted properly.
                renderer.Plain(head.Buffer);
                renderer.ReturnStringBuilder(head.Buffer);
            }
        }

        renderer.RenderHelpers(HelperTarget.HeadEnd);
    }

    private static void RenderBody(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.RenderHelpers(HelperTarget.BodyStart);

        foreach (var (_, value) in context.GlobalData)
        {
            if (value is BodyBuffer body && body.Buffer != null)
            {
                // Already formatted properly.
                renderer.Plain(body.Buffer);
                renderer.ReturnStringBuilder(body.Buffer);
            }
        }

        renderer.RenderHelpers(HelperTarget.BodyEnd);
    }
}
