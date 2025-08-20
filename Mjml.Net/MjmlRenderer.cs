using System.Text;
using Mjml.Net.Components;
using Mjml.Net.Components.Body;
using Mjml.Net.Components.Head;
using Mjml.Net.Helpers;
using Mjml.Net.Internal;

namespace Mjml.Net;

/// <summary>
/// Default implementation of the <see cref="IMjmlRenderer"/> interface.
/// </summary>
public sealed partial class MjmlRenderer : IMjmlRenderer
{
    private readonly Dictionary<string, Func<IComponent>> components = [];
    private readonly List<IHelper> helpers = [];

    /// <inheritdoc />
    public IReadOnlyCollection<Func<IComponent>> Components => components.Values;

    /// <inheritdoc />
    public IReadOnlyCollection<IHelper> Helpers => helpers;

    /// <summary>
    /// Initializes a new instance of the <see cref="MjmlRenderer"/> class.
    /// </summary>
    public MjmlRenderer()
    {
        Add<AccordionComponent>();
        Add<AccordionElementComponent>();
        Add<AccordionTextComponent>();
        Add<AccordionTitleComponent>();
        Add<AttributesComponent>();
        Add<BodyComponent>();
        Add<BreakpointComponent>();
        Add<ButtonComponent>();
        Add<CarouselComponent>();
        Add<CarouselImageComponent>();
        Add<ColumnComponent>();
        Add<DividerComponent>();
        Add<FontComponent>();
        Add<GroupComponent>();
        Add<HeadComponent>();
        Add<HeroComponent>();
        Add<IncludeComponent>();
        Add<ImageComponent>();
        Add<MsoButtonComponent>();
        Add<NavbarComponent>();
        Add<NavbarLinkComponent>();
        Add<PreviewComponent>();
        Add<RawComponent>();
        Add<RootComponent>();
        Add<SectionComponent>();
        Add<SocialComponent>();
        Add<SocialElementComponent>();
        Add<SpacerComponent>();
        Add<StyleComponent>();
        Add<TableComponent>();
        Add<TextComponent>();
        Add<TitleComponent>();
        Add<WrapperComponent>();

        Add(new FontHelper());
        Add(new PreviewHelper());
        Add(new StyleHelper());
        Add(new TitleHelper());
    }

    /// <inheritdoc />
    public IMjmlRenderer Add<T>() where T : IComponent, new()
    {
        components[new T().ComponentName] = () => new T();

        return this;
    }

    /// <inheritdoc />
    public IMjmlRenderer Add(IHelper helper)
    {
        helpers.Add(helper);

        return this;
    }

    /// <inheritdoc />
    public IMjmlRenderer ClearComponents()
    {
        components.Clear();

        return this;
    }

    /// <inheritdoc />
    public IMjmlRenderer ClearHelpers()
    {
        helpers.Clear();

        return this;
    }

    internal IComponent? CreateComponent(string name)
    {
        return components.GetValueOrDefault(name)?.Invoke();
    }

    /// <inheritdoc />
    public RenderResult Render(string mjml, MjmlOptions? options = null)
    {
        return RenderCore(mjml, false, options).Result;
    }

    /// <inheritdoc />
    public RenderResult Render(Stream mjml, MjmlOptions? options = null)
    {
        return Render(new StreamReader(mjml), options);
    }

    /// <inheritdoc />
    public RenderResult Render(TextReader mjml, MjmlOptions? options = null)
    {
        return RenderCore(mjml.ReadToEnd(), false, options).Result;
    }

    public async ValueTask<RenderResult> RenderAsync(string mjml, MjmlOptions? options = null,
        CancellationToken ct = default)
    {
        return await RenderCoreAsync(mjml, options, ct);
    }

    public async ValueTask<RenderResult> RenderAsync(Stream mjml, MjmlOptions? options = null,
        CancellationToken ct = default)
    {
        return await RenderAsync(new StreamReader(mjml), options, ct);
    }

    public async ValueTask<RenderResult> RenderAsync(TextReader mjml, MjmlOptions? options = null,
        CancellationToken ct = default)
    {
#if NET7_0_OR_GREATER
        return await RenderCoreAsync(await mjml.ReadToEndAsync(ct), options, ct);
#else
        return await RenderCoreAsync(await mjml.ReadToEndAsync(), options, ct);
#endif
    }

    private async ValueTask<RenderResult> RenderCoreAsync(string mjml, MjmlOptions? options,
        CancellationToken ct)
    {
#pragma warning disable MA0042 // Do not use blocking calls in an async method
        var (result, actualOptions) = RenderCore(mjml, true, options);
#pragma warning restore MA0042 // Do not use blocking calls in an async method

        if (actualOptions?.PostProcessors?.Length > 0 && !string.IsNullOrWhiteSpace(result.Html))
        {
            var html = result.Html;

            foreach (var processor in actualOptions.PostProcessors)
            {
                html = await processor.PostProcessAsync(html, actualOptions, ct);
            }

            result = new RenderResult(html, result.Errors);
        }

        return result;
    }

    private (RenderResult Result, MjmlOptions Options) RenderCore(string mjml, bool isAsync, MjmlOptions? options)
    {
        options ??= new MjmlOptions();

        var reader = new HtmlReaderWrapper(mjml);
        var context = DefaultPools.RenderContexts.Get();
        try
        {
            context.Setup(this, isAsync, options);
            context.StartBuffer();
            context.Read(reader, null, null);

            using var buffer = context.EndBuffer();

            return (new RenderResult(buffer.ToString()!, context.Validate()), options);
        }
        finally
        {
            DefaultPools.RenderContexts.Return(context);
        }
    }
}
