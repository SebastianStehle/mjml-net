using AngleSharp;
using AngleSharp.Css;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using Microsoft.Extensions.ObjectPool;
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

    private static readonly ObjectPool<IBrowsingContext> Contexts = new DefaultObjectPool<IBrowsingContext>(new ContextPolicy());

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
        // Parsing and serializing the whole document is expensive, so skip it when there is nothing to do.
        if (!inner.Any(x => x.ShouldProcess(html)))
        {
            return html;
        }

        // Creating a context is expensive, but it is not thread safe. Therefore reuse them over a pool.
        var context = Contexts.Get();
        try
        {
            using var document = await context.OpenAsync(req => req.Content(html), ct);

            foreach (var processor in inner)
            {
                await processor.ProcessAsync(document, options, ct);
            }

            return document.ToHtml();
        }
        finally
        {
            Contexts.Return(context);
        }
    }

    private sealed class ContextPolicy : PooledObjectPolicy<IBrowsingContext>
    {
        public override IBrowsingContext Create()
        {
            return BrowsingContext.New(HtmlConfiguration);
        }

        public override bool Return(IBrowsingContext obj)
        {
            return true;
        }
    }
}
