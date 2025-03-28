using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Text;

namespace Mjml.Net.Declarations;

internal class FallbackConverter(IValueConverter inner) : IValueConverter
{
    public ICssValue Convert(StringSource source)
    {
        var result = inner.Convert(source);

        result ??= new FallbackCssValue(source.TakeUntilClosed());

        return result;
    }
}
