namespace Mjml.Net.Helpers
{
    public sealed record Style(string Value, bool Inline = false)
    {
    }

    public sealed class StyleHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            if (data.Values.OfType<Style>().Any())
            {
                renderer.ElementStart("style")
                    .Attr("type", "text/css");

                foreach (var preview in data.Values.OfType<Style>())
                {
                    renderer.Content(preview.Value);
                }

                renderer.ElementEnd("style");
            }
        }
    }
}
