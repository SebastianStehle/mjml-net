using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct StyleProps
    {
        [Bind("inline")]
        public string? Inline;

        [BindText]
        public string? Text;
    }

    public sealed class StyleComponent : HeadComponentBase<StyleProps>
    {
        public override string ComponentName => "mj-style";

        public override bool NeedsContent => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new StyleProps(node);

            // Just in case that validation is disabled.
            if (props.Text != null)
            {
                // Allow multiple styles.
                renderer.SetGlobalData(Guid.NewGuid().ToString(), new Style(props.Text));
            }
        }
    }
}
