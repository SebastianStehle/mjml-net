using System.Collections.Concurrent;
using AngleSharp.Css;

namespace Mjml.Net.Declarations;

public class FallbackDeclarationFactory : IDeclarationFactory
{
    // The property names come from the CSS of the templates, so limit the cache to avoid unbounded growth.
    private const int MaxCachedDeclarations = 1024;

    // The declarations are immutable, so they can be shared between all factories and threads.
    private static readonly ConcurrentDictionary<string, DeclarationInfo> Cache = new ConcurrentDictionary<string, DeclarationInfo>(StringComparer.Ordinal);
    private static readonly DefaultDeclarationFactory DefaultFactory = new DefaultDeclarationFactory();

    public DeclarationInfo Create(string propertyName)
    {
        if (Cache.TryGetValue(propertyName, out var cached))
        {
            return cached;
        }

        var declaration = CreateCore(propertyName);

        // Counting is expensive for a concurrent dictionary, but only needed when a declaration is not cached yet.
        if (Cache.Count < MaxCachedDeclarations)
        {
            Cache.TryAdd(propertyName, declaration);
        }

        return declaration;
    }

    private static DeclarationInfo CreateCore(string propertyName)
    {
        var declaration = DefaultFactory.Create(propertyName);

        // AngleSharp only expands shorthands (e.g. padding, background) with its own converters. With a wrapped converter
        // the whole declaration is dropped, so keep them as they are.
        if (declaration.Flags.HasFlag(PropertyFlags.Shorthand))
        {
            return declaration;
        }

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
