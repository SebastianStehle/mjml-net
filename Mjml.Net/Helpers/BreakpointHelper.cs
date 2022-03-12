namespace Mjml.Net.Helpers
{
    public sealed class BreakpointHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, GlobalData data)
        {
            foreach (var (_, value) in data)
            {
                if (value is Breakpoint breakpoint)
                {
                    renderer.StartElement("style")
                        .Attr("type", "text/css");

                    renderer.Content($"@media only screen and (min-width:{breakpoint.Value}) {{");
                    renderer.Content($"  .mj-column-per-100 {{");
                    renderer.Content($"    width: 100% !important;");
                    renderer.Content($"    max-width: 100%;");
                    renderer.Content($"  }}");
                    renderer.Content($"}}");

                    renderer.EndElement("style");

                    renderer.StartElement("style")
                        .Attr("media", $"screen and (min-width:{breakpoint.Value})");

                    renderer.Content(".moz-text-html .mj-column-per-100 {");
                    renderer.Content("  width: 100% !important;");
                    renderer.Content("  max-width: 100%;");
                    renderer.Content("}}");

                    renderer.EndElement("style");
                }
            }
        }
    }
}
