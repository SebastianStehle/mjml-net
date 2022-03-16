using System.Xml;
using Mjml.Net.Components;
using Mjml.Net.Components.Body;
using Mjml.Net.Components.Head;
using Mjml.Net.Helpers;
using U8Xml;

namespace Mjml.Net
{
    public sealed class MjmlRenderer : IMjmlRenderer
    {
        private readonly Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();
        private readonly List<IHelper> helpers = new List<IHelper>();

        public IEnumerable<IHelper> Helpers => helpers;

        public MjmlRenderer()
        {
            Add(new AttributesComponent());
            Add(new BodyComponent());
            Add(new BreakpointComponent());
            Add(new DividerComponent());
            Add(new FontComponent());
            Add(new HeadComponent());
            Add(new HeroComponent());
            Add(new ImageComponent());
            Add(new PreviewComponent());
            Add(new RawComponent());
            Add(new RootComponent());
            Add(new SpacerComponent());
            Add(new StyleComponent());
            Add(new TitleComponent());
            Add(new TextComponent());
            Add(new ButtonComponent());

            Add(new BreakpointHelper());
            Add(new FontHelper());
            Add(new PreviewHelper());
            Add(new StyleHelper());
            Add(new TitleHelper());
        }

        public IMjmlRenderer Add(IComponent component)
        {
            components[component.ComponentName] = component;

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

        internal IComponent? GetComponent(string previousElement)
        {
            return components.GetValueOrDefault(previousElement);
        }

        public RenderResult Render(string mjml, MjmlOptions options = default)
        {
            return RenderCore(XmlParser.Parse(mjml), options);
        }

        public RenderResult Render(Stream mjml, MjmlOptions options = default)
        {
            return RenderCore(XmlParser.Parse(mjml), options);
        }

        private RenderResult RenderCore(XmlObject xml, MjmlOptions options)
        {
            using (xml)
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
}
