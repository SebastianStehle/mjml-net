using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct FontProperties
    {
        [Bind("name")]
        public string? Name;

        [Bind("href")]
        public string? Href;
    }

    public sealed class FontComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-font";

        public override bool SelfClosed => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var args = new FontProperties(node);

            // Just in case that validation is disabled.
            if (args.Href != null)
            {
                // The name does not really matter.
                var name = node.GetAttribute("name");

                renderer.SetGlobalData(name ?? Guid.NewGuid().ToString(), new Font(args.Href));
            }
        }
    }
}
