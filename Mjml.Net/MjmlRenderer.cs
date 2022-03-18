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
            Add<AttributesComponent>();
            Add<BodyComponent>();
            Add<BreakpointComponent>();
            Add<ButtonComponent>();
            Add<DividerComponent>();
            Add<FontComponent>();
            Add<GroupComponent>();
            Add<HeadComponent>();
            Add<HeroComponent>();
            Add<ImageComponent>();
            Add<PreviewComponent>();
            Add<RawComponent>();
            Add<RootComponent>();
            Add<SocialComponent>();
            Add<SocialElementComponent>();
            Add<SpacerComponent>();
            Add<StyleComponent>();
            Add<TextComponent>();
            Add<TitleComponent>();
            Add<SectionComponent>();

            Add(new FontHelper());
            Add(new PreviewHelper());
            Add(new StyleHelper());
            Add(new TitleHelper());
        }

        public IMjmlRenderer Add<T>() where T : IComponent, new()
        {
            components[new T().ComponentName] = () => new T();

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

        public RenderResult Render(string mjml, MjmlOptions? options = null)
        {
            var xml = XmlReader.Create(new StringReader(mjml));

            return Render(xml, options);
        }

        public RenderResult Render(Stream mjml, MjmlOptions? options = null)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        public RenderResult Render(TextReader mjml, MjmlOptions? options = null)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        private RenderResult Render(XmlReader xml, MjmlOptions? options)
        {
            var context = ObjectPools.Contexts.Get();
            try
            {
                context.Setup(this, options ?? new MjmlOptions());
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
