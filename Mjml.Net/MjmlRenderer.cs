﻿using System.Text;
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
    private readonly Dictionary<string, Func<IComponent>> components = new Dictionary<string, Func<IComponent>>();
    private readonly List<IHelper> helpers = new List<IHelper>();

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
        return RenderCore(mjml, options);
    }

    /// <inheritdoc />
    public RenderResult Render(Stream mjml, MjmlOptions? options = null)
    {
        return Render(new StreamReader(mjml), options);
    }

    /// <inheritdoc />
    public RenderResult Render(TextReader mjml, MjmlOptions? options = null)
    {
        return RenderCore(mjml.ReadToEnd(), options);
    }

    private RenderResult RenderCore(string mjml, MjmlOptions? options)
    {
        options ??= new MjmlOptions();

        var reader = new HtmlReaderWrapper(mjml);

        var context = DefaultPools.RenderContexts.Get();
        try
        {
            context.Setup(this, options);
            context.StartBuffer();
            context.Read(reader, null, null);

            StringBuilder? buffer = null;
            try
            {
                buffer = context.EndBuffer();

                return new RenderResult(buffer!.ToString()!, context.Validate());
            }
            finally
            {
                DefaultPools.StringBuilders.Return(buffer!);
            }
        }
        finally
        {
            DefaultPools.RenderContexts.Return(context);
        }
    }
}
