namespace Mjml.Net.Helpers
{
    public sealed record Preview(string Value)
    {
    }

    public sealed class PreviewHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data)
        {
            if (target != HelperTarget.HeadStart)
            {
                return;
            }

            if (data.Values.OfType<Preview>().Any())
            {
                renderer.ElementStart("div")
                    .Attr("style", "display:none;font-size:1px;color:#ffffff;line-height:1px;max-height:0px;max-width:0px;opacity:0;overflow:hidden;");

                foreach (var preview in data.Values.OfType<Preview>())
                {
                    renderer.Content(preview.Value);
                }

                renderer.ElementEnd("div");
            }
        }
    }
}
