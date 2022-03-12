namespace Mjml.Net
{
    /// <summary>
    /// A helper is responsible to set global data to tag, which will be mainly the head tag.
    /// </summary>
    /// <remarks>
    /// Used for fonts and styles.
    /// </remarks>
    public interface IHelper
    {
        /// <summary>
        /// Renders the global data.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        /// <param name="target">The target where the helpers are rendered.</param>
        /// <param name="data">The data to render.</param>
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data);
    }
}
