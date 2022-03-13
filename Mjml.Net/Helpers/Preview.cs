namespace Mjml.Net.Helpers
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record Preview(string Value)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
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
