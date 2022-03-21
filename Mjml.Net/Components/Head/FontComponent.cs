using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class FontComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-font";

        [Bind("name")]
        public string? Name;

        [Bind("href")]
        public string? Href;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Href != null)
            {
                context.SetGlobalData(Name ?? Href, new Font(Href));
            }
        }
    }
}
