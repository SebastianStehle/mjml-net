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
        if (result != null)
        {
            return result;
        }

        var value = source.Content;
        source.Next(value.Length);
        return new FallbackCssValue(value);
    }

    public ICssValue Merge(ICssValue[] values)
    {
        throw new NotImplementedException();
    }

    public ICssValue[] Split(ICssValue value)
    {
        throw new NotImplementedException();
    }
}
