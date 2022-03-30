using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mjml.Net
{
    public interface IHtmlStyleRenderer
    {
        /// <summary>
        /// Sets an style by name for the current element.
        /// </summary>
        /// <param name="name">The name of the style.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more styles.</returns>
        IHtmlStyleRenderer Style(string name, string? value);

        /// <summary>
        /// Sets an style by name for the current element.
        /// </summary>
        /// <param name="name">The name of the style.</param>
        /// <param name="value">The value of the attribute. If the value is null, it will be omitted.</param>
        /// <returns>The current instance to set more styles.</returns>
        IHtmlStyleRenderer Style(string name, [InterpolatedStringHandlerArgument("", "name")] ref StyleInterpolatedStringHandler value);

        internal StringBuilder StringBuilder { get; }

        internal void StartStyle(string name);
    }

    [InterpolatedStringHandler]
    public ref struct StyleInterpolatedStringHandler
    {
        private StringBuilder.AppendInterpolatedStringHandler inner;

        public StyleInterpolatedStringHandler(int literalLength, int formattedCount, IHtmlStyleRenderer writer, string name)
        {
            writer.StartStyle(name);

            inner = new StringBuilder.AppendInterpolatedStringHandler(literalLength, formattedCount, writer.StringBuilder, CultureInfo.InvariantCulture);
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
