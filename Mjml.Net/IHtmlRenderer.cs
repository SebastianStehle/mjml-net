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
        /// <param name="selfClosed">True, if the element is self closed.</param>
        /// <returns>
        /// A renderer to set attributes on the element.
        /// </returns>
        IElementHtmlWriter StartElement(string elementName, bool selfClosed = false);

        /// <summary>
        /// Ends an element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        void EndElement(string elementName);

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

    public interface IElementHtmlWriter : IElementClassWriter
    {
        /// <summary>
        /// Sets an attribute by name for the current element.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more attributes.</returns>
        IElementHtmlWriter Attr(string name, string? value);

        /// <summary>
        /// Sets an attribute by name for the current element.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>The current instance to set more attributes.</returns>
        IElementHtmlWriter Attr(string name, [InterpolatedStringHandlerArgument("", "name")] ref AttrInterpolatedStringHandler value);

        internal void StartAttr(string name);
    }

    [InterpolatedStringHandler]
    public ref struct AttrInterpolatedStringHandler
    {
        private StringBuilder.AppendInterpolatedStringHandler inner;

        public AttrInterpolatedStringHandler(int literalLength, int formattedCount, IElementHtmlWriter writer, string name)
        {
            writer.StartAttr(name);

            inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, writer.StringBuilder);
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

    public interface IElementStyleWriter
    {
        /// <summary>
        /// Sets an style by name for the current element.
        /// </summary>
        /// <param name="name">The name of the style.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more styles.</returns>
        IElementStyleWriter Style(string name, string? value);

        /// <summary>
        /// Sets an style by name for the current element.
        /// </summary>
        /// <param name="name">The name of the style.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more styles.</returns>
        IElementStyleWriter Style(string name, [InterpolatedStringHandlerArgument("", "name")] ref StyleInterpolatedStringHandler value);

        internal StringBuilder StringBuilder { get; }

        void StartStyle(string name);
    }

    [InterpolatedStringHandler]
    public ref struct StyleInterpolatedStringHandler
    {
        private StringBuilder.AppendInterpolatedStringHandler inner;

        public StyleInterpolatedStringHandler(int literalLength, int formattedCount, IElementStyleWriter writer, string name)
        {
            writer.StartStyle(name);

            inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, writer.StringBuilder);
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

    public interface IElementClassWriter : IElementStyleWriter
    {
        /// <summary>
        /// Adds a class name to the current element.
        /// </summary>
        /// <param name="value">The class name. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more class names.</returns>
        IElementClassWriter Class(string? value);

        /// <summary>
        /// Adds a class name to the current element.
        /// </summary>
        /// <param name="value">The class name. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more class names.</returns>
        IElementClassWriter Class([InterpolatedStringHandlerArgument("")] ref ClassNameInterpolatedStringHandler value);

        void StartClass();
    }

    [InterpolatedStringHandler]
    public ref struct ClassNameInterpolatedStringHandler
    {
        private StringBuilder.AppendInterpolatedStringHandler inner;

        public ClassNameInterpolatedStringHandler(int literalLength, int formattedCount, IElementClassWriter writer)
        {
            writer.StartClass();

            inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, writer.StringBuilder);
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
}
