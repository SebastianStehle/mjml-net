using Mjml.Net.Properties;

namespace Mjml.Net.Components
{
    public sealed class RootComponent : Component
    {
        public override string ComponentName => "mjml";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            RenderChildren(renderer, context);

            renderer.Content("<!doctype html>");
            renderer.Content("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">");
            renderer.Content(string.Empty);

            RenderHead(renderer, context);

            renderer.Content(string.Empty);

            RenderBody(renderer, context);

            renderer.Content(string.Empty);
            renderer.Content("</html>");
        }

        private static void RenderHead(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.ElementStart("head");

            // Helpers right after head.
            renderer.RenderHelpers(HelperTarget.HeadStart);

            // Add default things.
            renderer.Content(Resources.DefaultMeta);

            renderer.ElementStart("style").Attr("type", "text/css");
            renderer.Content(Resources.DefaultStyles);
            renderer.ElementEnd("style");

            renderer.Content(Resources.DefaultComments);

            // Already formatted properly.
            if (context.GlobalData.TryGetValue((typeof(string), "head"), out var head))
            {
                renderer.Plain(head.ToString());
            }

            // Helpers right before head ends.
            renderer.RenderHelpers(HelperTarget.HeadEnd);
            renderer.ElementEnd("head");
        }

        private static void RenderBody(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.ElementStart("body");

            // Helpers right after body.
            renderer.RenderHelpers(HelperTarget.BodyStart);

            // Already formatted properly.
            if (context.GlobalData.TryGetValue((typeof(string), "body"), out var head))
            {
                renderer.Plain(head.ToString());
            }

            // Helpers right before body ends.
            renderer.RenderHelpers(HelperTarget.BodyEnd);
            renderer.ElementEnd("body");
        }
    }
}
