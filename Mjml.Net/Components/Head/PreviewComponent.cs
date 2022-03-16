using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct PreviewProps
    {
        [BindText]
        public string? Text;
    }

    public sealed class PreviewComponent : HeadComponentBase<PreviewProps>
    {
        public override string ComponentName => "mj-preview";

        public override bool NeedsContent => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new PreviewProps(node);

            // Just in case that validation is disabled.
            if (props.Text != null)
            {
                // Allow multiple previews.
                renderer.SetGlobalData(Guid.NewGuid().ToString(), new Preview(props.Text));
            }
        }
    }
}
