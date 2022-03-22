using System.Runtime.CompilerServices;
using System.Text;

namespace Mjml.Net
{
    public interface IHtmlAttrRenderer : IHtmlClassRenderer
    {
        /// <summary>
        /// Sets an attribute by name for the current element.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more attributes.</returns>
        IHtmlAttrRenderer Attr(string name, string? value);

        /// <summary>
        /// Sets an attribute by name for the current element.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>The current instance to set more attributes.</returns>
        IHtmlAttrRenderer Attr(string name, [InterpolatedStringHandlerArgument("", "name")] ref AttrInterpolatedStringHandler value);

        internal void StartAttr(string name);
    }

    [InterpolatedStringHandler]
    public ref struct AttrInterpolatedStringHandler
    {
        private StringBuilder.AppendInterpolatedStringHandler inner;

        public AttrInterpolatedStringHandler(int literalLength, int formattedCount, IHtmlAttrRenderer renderer, string name)
        {
            renderer.StartAttr(name);

            inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, renderer.StringBuilder);
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
