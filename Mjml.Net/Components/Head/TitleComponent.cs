using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public sealed class TitleComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-title";

        public override bool NeedsContent => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var title = node.GetContent();

            // Just in case that validation is disabled.
            if (title != null)
            {
                renderer.SetGlobalData("default", new Title(title));
            }
        }
    }
}
