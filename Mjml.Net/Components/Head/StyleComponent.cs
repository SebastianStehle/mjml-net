using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head;

public partial class StyleComponent : HeadComponentBase
{
    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-style";

    [Bind("inline")]
    public string? Inline;

    [BindText]
    public InnerTextOrHtml? Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        // Just in case that validation is disabled.
        if (Text != null)
        {
            var style = Style.Static(Text);

            // Allow multiple styles.
            context.AddGlobalData(style);
        }
    }
}
