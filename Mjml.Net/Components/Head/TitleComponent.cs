using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head;

public partial class TitleComponent : HeadComponentBase
{
    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-title";

    [BindText]
    public InnerTextOrHtml? Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        // Just in case that validation is disabled.
        if (Text != null)
        {
            context.SetGlobalData("default", new Title(Text));
        }
    }
}
