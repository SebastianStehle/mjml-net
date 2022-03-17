using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class StyleComponent : HeadComponentBase
    {
        public override ComponentType Type => ComponentType.Text;

        public override string ComponentName => "mj-style";

        [Bind("inline")]
        public string? Inline;

        [BindText]
        public string? Text;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Text != null)
            {
                // Allow multiple styles.
                context.SetGlobalData(Guid.NewGuid().ToString(), new Style(Text));
            }
        }
    }
}
