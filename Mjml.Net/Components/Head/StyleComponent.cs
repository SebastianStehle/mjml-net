using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class StyleProps
    {
        [Bind("inline")]
        public string? Inline;

        [BindText]
        public string? Text;
    }

    public sealed class StyleComponent : HeadComponentBase<StyleProps>
    {
        public override ComponentType Type => ComponentType.Text;

        public override string Name => "mj-style";

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Props.Text != null)
            {
                // Allow multiple styles.
                context.SetGlobalData(Guid.NewGuid().ToString(), new Style(Props.Text));
            }
        }
    }
}
