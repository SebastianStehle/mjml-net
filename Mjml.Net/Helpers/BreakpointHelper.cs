namespace Mjml.Net.Helpers
{
    public sealed class BreakpointHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data)
        {
            if (target != HelperTarget.HeadEnd)
            {
                return;
            }

            foreach (var (_, value) in data)
            {
                if (value is Breakpoint breakpoint)
                {
                    renderer.ElementStart("style")
                        .Attr("type", "text/css");

                    renderer.Content($"@media only screen and (min-width:{breakpoint.Value}) {{");
                    renderer.Content($"  .mj-column-per-100 {{");
                    renderer.Content($"    width: 100% !important;");
                    renderer.Content($"    max-width: 100%;");
                    renderer.Content($"  }}");
                    renderer.Content($"}}");

                    renderer.ElementEnd("style");

                    renderer.ElementStart("style")
                        .Attr("media", $"screen and (min-width:{breakpoint.Value})");

                    renderer.Content(".moz-text-html .mj-column-per-100 {");
                    renderer.Content("  width: 100% !important;");
                    renderer.Content("  max-width: 100%;");
                    renderer.Content("}}");

                    renderer.ElementEnd("style");
                }
            }
        }
    }
}
