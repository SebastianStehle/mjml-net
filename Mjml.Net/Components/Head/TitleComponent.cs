using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial struct TitleProps
    {
        [BindText]
        public string? Text;
    }

    public sealed class TitleComponent : HeadComponentBase<TitleProps>
    {
        public override string ComponentName => "mj-title";

        public override bool NeedsContent => true;

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var props = new TitleProps(node);

            // Just in case that validation is disabled.
            if (props.Text != null)
            {
                renderer.SetGlobalData("default", new Title(props.Text));
            }
        }
    }
}
