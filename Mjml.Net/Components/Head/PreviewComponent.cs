using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class PreviewProps
    {
        [BindText]
        public string? Text;
    }

    public sealed class PreviewComponent : HeadComponentBase<PreviewProps>
    {
        public override ComponentType Type => ComponentType.Text;

        public override string Name => "mj-preview";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Props.Text != null)
            {
                // Allow multiple previews.
                context.SetGlobalData(Guid.NewGuid().ToString(), new Preview(Props.Text));
            }
        }
    }
}
