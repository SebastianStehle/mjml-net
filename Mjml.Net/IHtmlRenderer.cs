namespace Mjml.Net
{
    /// <summary>
    /// Renders html for MJML.
    /// </summary>
    public interface IHtmlRenderer : IContext
    {
        /// <summary>
        /// Starts a new element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="selfClosed">True, if the element is self closed.</param>
        /// <returns>
        /// A renderer to set attributes on the element.
        /// </returns>
        IElementHtmlRenderer ElementStart(string elementName, bool selfClosed = false);

        /// <summary>
        /// Ends an element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        void ElementEnd(string elementName);

        /// <summary>
        /// Renders a plain value.
        /// </summary>
        /// <param name="value">The value to render.</param>
        /// <param name="appendLine">True to append a new line.</param>
        void Plain(string? value, bool appendLine = true);

        /// <summary>
        /// Renders a plain value.
        /// </summary>
        /// <param name="value">The value to render.</param>
        /// <param name="appendLine">True to append a new line.</param>
        void Plain(ReadOnlySpan<char> value, bool appendLine = true);

        /// <summary>
        /// Renders the content of an element.
        /// </summary>
        /// <param name="value">The value to render.</param>
        /// <param name="appendLine">True to append a new line.</param>
        void Content(string? value, bool appendLine = true);

        /// <summary>
        /// Render all helpers.
        /// </summary>
        /// <param name="target">Optional target for the helpers.</param>
        void RenderHelpers(HelperTarget target);

        /// <summary>
        /// Renders everything into a temporary buffer and adds the buffer to the stack.
        /// </summary>
        void BufferStart();

        /// <summary>
        /// Removes the buffer from the stack and returns the content.
        /// </summary>
        /// <returns>The buffer content.</returns>
        string BufferFlush();
    }

    public interface IElementHtmlRenderer : IElementClassWriter
    {
        /// <summary>
        /// Sets an attribute by name for the current element.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more attributes.</returns>
        IElementHtmlRenderer Attr(string name, string? value);
    }

    public interface IElementStyleWriter
    {
        /// <summary>
        /// Sets an style by name for the current element.
        /// </summary>
        /// <param name="name">The name of the style.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more styles.</returns>
        IElementStyleWriter Style(string name, string? value);
    }

    public interface IElementClassWriter : IElementStyleWriter
    {
        /// <summary>
        /// Adds a class name to the current element.
        /// </summary>
        /// <param name="value">The class name. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more class names.</returns>
        IElementClassWriter Class(string? value);
    }
}
