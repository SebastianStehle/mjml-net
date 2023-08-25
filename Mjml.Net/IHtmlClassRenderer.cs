using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mjml.Net;

public interface IHtmlClassRenderer : IHtmlStyleRenderer
{
    /// <summary>
    /// Adds a class name to the current element.
    /// </summary>
    /// <param name="value">The class name. If the value is null, it will be omitted.</param>
    /// <returns>The current instance to set more class names.</returns>
    IHtmlClassRenderer Class(string? value);

    /// <summary>
    /// Adds a class name to the current element.
    /// </summary>
    /// <param name="value">The class name. If the value is null, it will be omitted.</param>
    /// <returns>The current instance to set more class names.</returns>
    IHtmlClassRenderer Class([InterpolatedStringHandlerArgument("")] ref ClassNameInterpolatedStringHandler value);

    internal IHtmlClassRenderer StartClass();
}

[InterpolatedStringHandler]
public ref struct ClassNameInterpolatedStringHandler
{
    private StringBuilder.AppendInterpolatedStringHandler inner;

    public ClassNameInterpolatedStringHandler(int literalLength, int formattedCount, IHtmlClassRenderer renderer)
    {
        renderer.StartClass();

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
