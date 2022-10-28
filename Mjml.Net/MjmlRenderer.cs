using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.ObjectPool;
using Mjml.Net.Components;
using Mjml.Net.Components.Body;
using Mjml.Net.Components.Head;
using Mjml.Net.Helpers;

namespace Mjml.Net
{
    /// <summary>
    /// Default implementation of the <see cref="IMjmlRenderer"/> interface.
    /// </summary>
    public sealed class MjmlRenderer : IMjmlRenderer
    {
        private readonly Regex attributeRegex = new Regex(@"\s*(?<Name>[a-z0-9]*)=""(?<Value>.*)""([\s]|>)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private readonly ObjectPool<StringBuilder> poolOfStringBuilders = new DefaultObjectPool<StringBuilder>(new StringBuilderPooledObjectPolicy());
        private readonly ObjectPool<MjmlRenderContext> poolOfContexts = new DefaultObjectPool<MjmlRenderContext>(new MjmlRenderContextPolicy());
        private readonly Dictionary<string, Func<IComponent>> components = new Dictionary<string, Func<IComponent>>();
        private readonly List<IHelper> helpers = new List<IHelper>();

        private sealed class MjmlRenderContextPolicy : PooledObjectPolicy<MjmlRenderContext>
        {
            public override MjmlRenderContext Create()
            {
                return new MjmlRenderContext();
            }

            public override bool Return(MjmlRenderContext obj)
            {
                obj.Clear();

                return true;
            }
        }

        /// <summary>
        /// Provides a list of all registered helpers.
        /// </summary>
        public IEnumerable<IHelper> Helpers => helpers;

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
            if (options?.Lax == true)
            {
                mjml = FixXML(mjml, options);
            }

            var xml = XmlReader.Create(new StringReader(mjml));

            return Render(xml, options);
        }

        /// <inheritdoc />
        public RenderResult Render(Stream mjml, MjmlOptions? options = null)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        /// <inheritdoc />
        public RenderResult Render(TextReader mjml, MjmlOptions? options = null)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        public string FixXML(string mjml, MjmlOptions? options = null)
        {
            options ??= new MjmlOptions();

            foreach (var (key, value) in options.XmlEntities)
            {
                mjml = mjml.Replace(key, value, StringComparison.OrdinalIgnoreCase);
            }

            mjml = FixLineBreakTags(mjml);

            mjml = attributeRegex.Replace(mjml, match =>
            {
                var raw = match.ToString();

                var value = match.Groups["Value"].Value;

                if (!value.Contains('&', StringComparison.OrdinalIgnoreCase))
                {
                    return raw;
                }

                value = value.Replace("&", "&amp;", StringComparison.OrdinalIgnoreCase);

                return $" {match.Groups["Name"].Value}=\"{value}\"{raw[^1]}";
            });

            static string FixLineBreakTags(string input)
            {
                var currentIndex = 0;

                const string OpeningTag = "<br>";

                while (true)
                {
                    var indexOfBr = input.IndexOf(OpeningTag, currentIndex, StringComparison.OrdinalIgnoreCase);

                    if (indexOfBr >= 0)
                    {
                        static bool HasCloseTag(string input, int startIndex, string closing)
                        {
                            var closeIndex = input.IndexOf(closing, startIndex, StringComparison.OrdinalIgnoreCase);

                            if (closeIndex < 0)
                            {
                                return false;
                            }

                            for (var i = startIndex; i < closeIndex; i++)
                            {
                                var c = input[i];

                                if (c != ' ' && c != '\r' && c != '\n')
                                {
                                    return false;
                                }
                            }

                            return true;
                        }

                        currentIndex = indexOfBr + 4;

                        if (HasCloseTag(input, currentIndex, "</br>") ||
                            HasCloseTag(input, currentIndex, "</ br>"))
                        {
                            continue;
                        }

                        input = input.Remove(indexOfBr, OpeningTag.Length);
                        input = input.Insert(indexOfBr, "<br />");
                    }
                    else
                    {
                        break;
                    }
                }

                return input;
            }

            return mjml;
        }

        internal StringBuilder GetStringBuilder()
        {
            return poolOfStringBuilders.Get();
        }

        internal void Return(StringBuilder stringBuilder)
        {
            poolOfStringBuilders.Return(stringBuilder);
        }

        private RenderResult Render(XmlReader xml, MjmlOptions? options)
        {
            var context = poolOfContexts.Get();
            try
            {
                context.Setup(this, options ?? new MjmlOptions());
                context.StartBuffer();
                context.ReadXml(xml, null);

                StringBuilder? buffer = null;
                try
                {
                    buffer = context.EndBuffer();

                    return new RenderResult(buffer!.ToString()!, context.Validate());
                }
                finally
                {
                    Return(buffer!);
                }
            }
            finally
            {
                poolOfContexts.Return(context);
            }
        }
    }
}
