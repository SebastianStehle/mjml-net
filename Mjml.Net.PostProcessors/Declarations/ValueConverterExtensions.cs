using AngleSharp.Css;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Text;

namespace Mjml.Net.Declarations;

internal static class ValueConverterExtensions
{
    public static ICssValue? Convert(this IValueConverter converter, string value)
    {
        var source = new StringSource(value);
        source.SkipSpacesAndComments();
        var varRefs = source.ParseVars();

        if (varRefs == null)
        {
            var result = converter.Convert(source);
            source.SkipSpacesAndComments();
            return source.IsDone ? result : null;
        }

        return varRefs;
    }
}
