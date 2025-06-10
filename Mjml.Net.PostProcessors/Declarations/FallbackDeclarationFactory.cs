using AngleSharp.Css;

namespace Mjml.Net.Declarations;

public class FallbackDeclarationFactory : IDeclarationFactory
{
    private readonly DefaultDeclarationFactory defaultFactory = new DefaultDeclarationFactory();

    public DeclarationInfo Create(string propertyName)
    {
        var declaration = defaultFactory.Create(propertyName);

        var converter =
            declaration.Converter is IValueAggregator aggregator ?
            new FallbackCssValueConverterWithAggregate(declaration.Converter, aggregator) :
            new FallbackCssValueConverter(declaration.Converter);

        var withConverter = new DeclarationInfo(
            declaration.Name,
            converter,
            declaration.Flags,
            declaration.InitialValue,
            declaration.Shorthands,
            declaration.Longhands);

        return withConverter;
    }
}
