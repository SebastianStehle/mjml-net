using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mjml.Net;

/// <summary>
/// Renders HTML for MJML.
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
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Plain(ReadOnlySpan<char> value);

    /// <summary>
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Plain(InnerTextOrHtml value);

    /// <summary>
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Plain(IBuffer value);

    /// <summary>
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Content(string? value);

    /// <summary>
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Content(InnerTextOrHtml? value);

    /// <summary>
    /// Renders a text.
    /// </summary>
    /// <param name="value">The value to render.</param>
    void Content([InterpolatedStringHandlerArgument("")] ref TextInterpolatedStringHandler value);

    /// <summary>
    /// Render all helpers.
    /// </summary>
    /// <param name="target">Optional target for the helpers.</param>
    void RenderHelpers(HelperTarget target);

    /// <summary>
    /// Renders everything into a temporary buffer and adds the buffer to the stack.
    /// </summary>
    void StartBuffer();

    /// <summary>
    /// Removes the buffer from the stack and returns the content.
    /// </summary>
    /// <returns>The buffer content.</returns>
    IBuffer EndBuffer();

    internal StringBuilder StringBuilder { get; }

    internal void StartText();
}

[InterpolatedStringHandler]
public ref struct TextInterpolatedStringHandler
{
    private StringBuilder.AppendInterpolatedStringHandler inner;

    public TextInterpolatedStringHandler(int literalLength, int formattedCount, IHtmlRenderer renderer)
    {
        renderer.StartText();

        inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, renderer.StringBuilder, CultureInfo.InvariantCulture);
    }

    public void AppendLiteral(string value)
    {
        inner.AppendLiteral(value);
    }

    public void AppendFormatted<T>(T value)
    {
        inner.AppendFormatted(value);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        inner.AppendFormatted(value, format);
    }

    public void AppendFormatted<T>(T value, int alignment)
    {
        inner.AppendFormatted(value, alignment);
    }

    public void AppendFormatted<T>(T value, int alignment, string? format)
    {
        inner.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(ReadOnlySpan<char> value)
    {
        inner.AppendFormatted(value);
    }

    public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, string? format = null)
    {
        inner.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(string? value)
    {
        inner.AppendFormatted(value);
    }

    public void AppendFormatted(string? value, int alignment = 0, string? format = null)
    {
        inner.AppendFormatted(value, alignment, format);
    }

    public void AppendFormatted(object? value, int alignment = 0, string? format = null)
    {
        inner.AppendFormatted(value, alignment, format);
    }
}
