using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head;

public partial class StyleComponent : HeadComponentBase
{
    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-style";

    [Bind("inline")]
    public string? Inline;

    [BindText]
    public string? Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        // Just in case that validation is disabled.
        if (Text != null)
        {
            // Allow multiple styles.
            context.SetGlobalData(Text, Style.Static(Text));
        }
    }
}
