namespace Mjml.Net.Helpers
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record Style(string Value, bool Inline = false)
    {
    }

    public sealed record DynamicStyle(Func<GlobalContext, string> Renderer)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
    {
    }

    public sealed class StyleHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalContext context)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            if (context.GlobalData.Values.Any(x => x is Style || x is DynamicStyle))
            {
                renderer.ElementStart("style")
                    .Attr("type", "text/css");

                foreach (var style in context.GlobalData.Values.OfType<Style>())
                {
                    renderer.Content(style.Value);
                }

                foreach (var style in context.GlobalData.Values.OfType<DynamicStyle>())
                {
                    renderer.Content(style.Renderer(context));
                }

                renderer.ElementEnd("style");
            }
        }
    }
}
