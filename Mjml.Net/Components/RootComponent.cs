using Mjml.Net.Properties;

namespace Mjml.Net.Components
{
    public sealed class ColumnComponent : IComponent
    {
        public string ComponentName => "mjml";

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.RenderChildren();

            renderer.Plain("<!doctype html>");
            renderer.Plain("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">");
            renderer.Plain(string.Empty);

            renderer.ElementStart("head");

            // Helpers right after head.
            renderer.RenderHelpers(HelperTarget.HeadStart);

            // Add default things.
            renderer.Content(Resources.DefaultMeta);
            renderer.Content(Resources.DefaultStyles);
            renderer.Content(Resources.DefaultComments);

            // Already formatted properly.
            renderer.Plain(renderer.GetContext("head") as string);

            // Helpers right before head ends.
            renderer.RenderHelpers(HelperTarget.HeadEnd);
            renderer.ElementEnd("head");

            renderer.Plain(string.Empty);

            renderer.ElementStart("body");

            // Helpers right after body.
            renderer.RenderHelpers(HelperTarget.BodyStart);

            // Already formatted properly.
            renderer.Plain(renderer.GetContext("body") as string);

            // Helpers right before body ends.
            renderer.RenderHelpers(HelperTarget.BodyEnd);
            renderer.ElementEnd("body");

            renderer.Plain(string.Empty);

            renderer.Plain("</html>");
        }
    }
}
