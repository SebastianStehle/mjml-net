using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class PreviewComponent : HeadComponentBase
    {
        public override ContentType ContentType => ContentType.Text;

        public override string ComponentName => "mj-preview";

        [BindText]
        public string? Text;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Text != null)
            {
                // Allow multiple previews.
                context.SetGlobalData(Guid.NewGuid().ToString(), new Preview(Text));
            }
        }
    }
}
