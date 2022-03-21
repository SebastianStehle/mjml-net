using System.Runtime.CompilerServices;
using System.Text;

namespace Mjml.Net
{
    /// <summary>
    /// Renders html for MJML.
    /// </summary>
    public interface IHtmlRenderer
    {
        /// <summary>
        /// Starts a new element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="close">True, if the element is self closed.</param>
        /// <returns>
        /// A renderer to set attributes on the element.
        /// </returns>
        IHtmlAttrRenderer StartElement(string elementName, bool close = false);

        /// <summary>
        /// Ends an element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        void EndElement(string elementName);

        /// <summary>
        /// Starts to write a conditional.
        /// </summary>
        /// <param name="content">The conditional to write.</param>
        void StartConditional(string content);

        /// <summary>
        /// Ends a conditional.
        /// </summary>
        /// <param name="content">The conditional to write.</param>
        void EndConditional(string content);

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
        /// Renders a plain value.
        /// </summary>
        /// <param name="value">The value to render.</param>
        /// <param name="appendLine">True to append a new line.</param>
        void Plain(StringBuilder value, bool appendLine = true);

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
        StringBuilder? BufferFlush();
    }
}
