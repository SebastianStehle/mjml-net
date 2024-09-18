using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head;

public partial class StyleComponent : HeadComponentBase
{
    public override ContentType ContentType => ContentType.Text;

    public override string ComponentName => "mj-style";

    [Bind("inline", BindType.Inline)]
    public string? Inline;

    [BindText]
    public InnerTextOrHtml? Text;

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        // Just in case that validation is disabled.
        if (Text != null)
        {
            var isInline = string.Equals(Inline, "inline", StringComparison.OrdinalIgnoreCase);

            var style = Style.Static(Text, isInline);

            // Allow multiple styles.
            context.AddGlobalData(style);
        }
    }
}
