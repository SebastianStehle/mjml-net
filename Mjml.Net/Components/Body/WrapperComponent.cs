using Mjml.Net.Extensions;

namespace Mjml.Net.Components.Body
{
    public partial class WrapperComponent : SectionComponent
    {
        public override string ComponentName => "mj-wrapper";

        protected override void RenderWrappedChildren(IHtmlRenderer renderer, GlobalContext context)
        {
            foreach (var child in ChildNodes)
            {
                if (child.Raw)
                {
                    renderer.EndConditional("<![endif]-->");
                    child.Render(renderer, context);
                    renderer.StartConditional("<!--[if mso | IE]>");
                }
                else
                {
                    renderer.StartElement("tr");
                    renderer.StartElement("td")
                        .Attr("align", child.GetAttribute("align"))
                        .Attr("width", $"{ActualWidth}px")
                        .Classes(child.GetAttribute("css-class"), "outlook");
                    renderer.EndConditional("<![endif]-->");

                    child.Render(renderer, context);

                    renderer.StartConditional("<!--[if mso | IE]>");
                    renderer.EndElement("td");
                    renderer.EndElement("tr");
                }
            }
        }
    }
}
