using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public sealed class PreviewComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-preview";

        public bool NeedsContent => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var preview = node.GetContent();

            // Just in case that validation is disabled.
            if (preview != null)
            {
                // Allow multiple previews.
                renderer.SetGlobalData(Guid.NewGuid().ToString(), new Preview(preview));
            }
        }
    }
}
