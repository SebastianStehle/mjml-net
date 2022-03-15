using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct FontProps
    {
        [Bind("name")]
        public string? Name;

        [Bind("href")]
        public string? Href;
    }

    public sealed class FontComponent : HeadComponentBase<FontProps>
    {
        public override string ComponentName => "mj-font";

        public override bool SelfClosed => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new FontProps(node);

            // Just in case that validation is disabled.
            if (props.Href != null)
            {
                // The name does not really matter.
                var name = props.Name;

                renderer.SetGlobalData(name ?? Guid.NewGuid().ToString(), new Font(props.Href));
            }
        }
    }
}
