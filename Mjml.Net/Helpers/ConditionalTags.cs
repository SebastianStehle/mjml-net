namespace Mjml.Net.Helpers
{
    internal static class ConditionalTags
    {
        internal const string StartConditional = "<!--[if mso | IE]>";
        internal const string StartMsoConditional = "<!--[if mso]>";
        internal const string EndConditional = "<![endif]-->";

        internal const string StartNotConditional = "<!--[if !mso | IE]><!-->";
        internal const string StartNotMsoConditional = "<!--[if !mso><!-->";
        internal const string EndNotConditional = "<!--<![endif]-->";
    }
}
