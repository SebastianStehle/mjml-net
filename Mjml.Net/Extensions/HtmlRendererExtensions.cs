using Mjml.Net.Helpers;

namespace Mjml.Net.Extensions
{
    public static class HtmlRendererExtensions
    {
        /// <summary>
        /// Renders the start of conditional tag to target Internet Explorer and Mso.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param>
        public static void StartConditionalTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.StartConditional);
        }

        /// <summary>
        /// Renders the start of conditional tag to target Mso.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param>
        public static void StartConditionalMsoTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.StartMsoConditional);
        }

        /// <summary>
        /// Renders the start of conditional tag to NOT target Internet Explorer and Mso.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param> 
        public static void StartConditionalNotTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.StartNotConditional);
        }

        /// <summary>
        /// Renders the start of conditional tag to NOT target Mso.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param>
        public static void StartConditionalNotMsoTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.StartNotMsoConditional);
        }

        /// <summary>
        /// Renders the end of conditional tag.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param>
        public static void EndConditionalTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.EndConditional);
        }

        /// <summary>
        /// Renders the end of an NOT conditional tag.
        /// </summary>
        /// <param name="renderer">The Html renderer to output too</param>
        public static void EndConditionalNotTag(this IHtmlRenderer renderer)
        {
            renderer.Content(ConditionalTags.EndNotConditional);
        }
    }
}
