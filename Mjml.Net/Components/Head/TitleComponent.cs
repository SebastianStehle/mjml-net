using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public partial class TitleProps
    {
        [BindText]
        public string? Text;
    }

    public sealed class TitleComponent : HeadComponentBase<TitleProps>
    {
        public override ComponentType Type => ComponentType.Text;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            // Just in case that validation is disabled.
            if (Props.Text != null)
            {
                context.SetGlobalData("default", new Title(Props.Text));
            }
        }
    }
}
