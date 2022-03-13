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

            renderer.RenderHelpers(HelperTarget.HeadStart);
            renderer.Plain(renderer.GetContext("head") as string);
            renderer.RenderHelpers(HelperTarget.HeadEnd);

            renderer.RenderHelpers(HelperTarget.BodyStart);
            renderer.Plain(renderer.GetContext("body") as string);
            renderer.RenderHelpers(HelperTarget.BodyEnd);
        }
    }
}
