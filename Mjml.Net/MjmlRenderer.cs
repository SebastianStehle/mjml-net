using System.Xml;
using Mjml.Net.Components;
using Mjml.Net.Components.Body;
using Mjml.Net.Components.Head;
using Mjml.Net.Helpers;

namespace Mjml.Net
{
    public sealed class MjmlRenderer : IMjmlRenderer
    {
        private readonly Dictionary<string, Func<IComponent>> components = new Dictionary<string, Func<IComponent>>();
        private readonly List<IHelper> helpers = new List<IHelper>();

        public IEnumerable<IHelper> Helpers => helpers;

        public MjmlRenderer()
        {
            Add<AttributesComponent>("mj-attributes");
            Add<BodyComponent>("mj-body");
            Add<BreakpointComponent>("mj-breakpoint");
            Add<ButtonComponent>("mj-button");
            Add<DividerComponent>("mj-divider");
            Add<FontComponent>("mj-font");
            Add<HeadComponent>("mj-head");
            Add<HeroComponent>("mj-hero");
            Add<ImageComponent>("mj-image");
            Add<PreviewComponent>("mj-preview");
            Add<RawComponent>("mj-raw");
            Add<RootComponent>("mjml");
            Add<SpacerComponent>("mj-spacer");
            Add<StyleComponent>("mj-style");
            Add<TextComponent>("mj-text");
            Add<TitleComponent>("mj-title");

            Add(new BreakpointHelper());
            Add(new FontHelper());
            Add(new PreviewHelper());
            Add(new StyleHelper());
            Add(new TitleHelper());
        }

        public IMjmlRenderer Add<T>(string name) where T : IComponent, new()
        {
            components[name] = () => new T();

            return this;
        }

        public IMjmlRenderer Add(IHelper helper)
        {
            helpers.Add(helper);

            return this;
        }

        public IMjmlRenderer ClearComponents()
        {
            components.Clear();

            return this;
        }

        public IMjmlRenderer ClearHelpers()
        {
            helpers.Clear();

            return this;
        }

        internal IComponent? CreateComponent(string name)
        {
            return components.GetValueOrDefault(name)?.Invoke();
        }

        public RenderResult Render(string mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(new StringReader(mjml));

            return Render(xml, options);
        }

        public RenderResult Render(Stream mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        public RenderResult Render(TextReader mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        private RenderResult Render(XmlReader xml, MjmlOptions options)
        {
            var context = ObjectPools.Contexts.Get();
            try
            {
                context.Setup(this, options);
                context.BufferStart();
                context.Read(xml);

                return new RenderResult(context.BufferFlush(), context.Validate());
            }
            finally
            {
                ObjectPools.Contexts.Return(context);
            }
        }
    }
}
