using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class FontProps
    {
        [Bind("name")]
        public string? Name;

        [Bind("href")]
        public string? Href;
    }

    public sealed class FontComponent : HeadComponentBase<FontProps>
    {
        public override string Name => "mj-font";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Props.Href != null)
            {
                context.SetGlobalData(Props.Name ?? Guid.NewGuid().ToString(), new Font(Props.Href));
            }
        }
    }
}
