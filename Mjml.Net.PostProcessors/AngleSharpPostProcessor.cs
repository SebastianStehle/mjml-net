using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using Mjml.Net.Declarations;

namespace Mjml.Net;

public sealed class AngleSharpPostProcessor : IPostProcessor, INestingPostProcessor
{
    private static readonly IConfiguration HtmlConfiguration =
        Configuration.Default
            .WithCss(new CssParserOptions
            {
                IsIncludingUnknownDeclarations = true,
                IsIncludingUnknownRules = true
            })
            .WithRenderDevice(new DefaultRenderDevice { FontSize = -1 })
            .Without<IDeclarationFactory>()
            .Without<ICssDefaultStyleSheetProvider>()
            .With<IDeclarationFactory>(_ => new FallbackDeclarationFactory());

    public static readonly IPostProcessor Default = new AngleSharpPostProcessor(new InlineCssPostProcessor(), new AttributesPostProcessor());

    private readonly IAngleSharpPostProcessor[] inner;

    public bool Has<T>()
    {
        return inner.Any(x => x is T);
    }

    public AngleSharpPostProcessor(params IAngleSharpPostProcessor[] inner)
    {
        this.inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async ValueTask<string> PostProcessAsync(string html, MjmlOptions options,
        CancellationToken ct)
    {
        var document = await ParseAsync(html, ct);

        foreach (var processor in inner)
        {
            await processor.ProcessAsync(document, options, ct);
        }

        var result = document.ToHtml();

        return result;
    }

    private static async Task<IDocument> ParseAsync(string html, CancellationToken ct)
    {
        var context = BrowsingContext.New(HtmlConfiguration);

        return await context.OpenAsync(req => req.Content(html), ct);
    }
}
