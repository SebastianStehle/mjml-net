using AngleSharp.Css;
using AngleSharp.Dom;
using AngleSharp.Io;

namespace Mjml.Net;

internal sealed class InlineOnlyStylingService : IStylingService
{
    private readonly IStylingService inner = new CssStylingService();

    public bool SupportsType(string mimeType)
    {
        return inner.SupportsType(mimeType);
    }

    public Task<IStyleSheet> ParseStylesheetAsync(IResponse response, StyleOptions options, CancellationToken cancel)
    {
        // Only inline style sheets are applied by the inliner. Parsing the other sheets (e.g. media queries) is expensive.
        if (options.Element == null || !InlineCssPostProcessor.IsInline(options.Element))
        {
            return Task.FromResult<IStyleSheet>(null!);
        }

        return inner.ParseStylesheetAsync(response, options, cancel);
    }
}
