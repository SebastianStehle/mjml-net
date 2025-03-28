using AngleSharp.Css.Dom;
using AngleSharp.Css.Values;

namespace Mjml.Net.Declarations;

internal sealed class FallbackCssValue(string text) : ICssValue
{
    public string CssText => text;

    ICssValue? ICssValue.Compute(ICssComputeContext context)
    {
        var converter = context.Converter;

        if (converter is not null && converter is not FallbackCssValueConverter)
        {
            var value = converter.Convert(text);
            return value?.Compute(context);
        }

        return null;
    }

    public bool Equals(ICssValue? other)
    {
        return other is FallbackCssValue f && f.CssText == CssText;
    }
}
