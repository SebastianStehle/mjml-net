namespace Mjml.Net
{
    /// <summary>
    /// Defines what kind of content the component needs.
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// The content is rendered raw.
        /// </summary>
        Raw,

        /// <summary>
        /// The component is complex and has other components as children.
        /// </summary>
        Complex,

        /// <summary>
        /// The content needs text contents.
        /// </summary>
        Text
    }
}
