using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Text;

namespace Mjml.Net.Declarations;

internal sealed class FallbackCssValueConverter : IValueConverter
{
    public ICssValue Convert(StringSource source)
    {
        var value = source.Content;
        source.Next(value.Length);

        return new FallbackCssValue(value);
    }
}
