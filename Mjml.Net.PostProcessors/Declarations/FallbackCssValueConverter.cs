using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Text;

namespace Mjml.Net.Declarations;

internal class FallbackCssValueConverter(IValueConverter inner)
    : IValueConverter
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
}

internal sealed class FallbackCssValueConverterWithAggregate(IValueConverter inner, IValueAggregator innerAggregator)
    : FallbackCssValueConverter(inner), IValueAggregator
{
    public ICssValue Merge(ICssValue[] values)
    {
        return innerAggregator.Merge(values);
    }

    public ICssValue[] Split(ICssValue value)
    {
        return innerAggregator.Split(value);
    }
}
