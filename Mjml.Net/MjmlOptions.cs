using System.Globalization;
using System.Xml;
using Mjml.Net.Helpers;

namespace Mjml.Net
{
    /// <summary>
    /// Provides options to configure the MJML rendering process.
    /// </summary>
    public sealed record MjmlOptions
    {
        private static readonly XmlParserContext DefaultParserContext;

        /// <summary>
        /// Gets the default font.
        /// </summary>
        public static readonly Dictionary<string, Font> DefaultFonts = new Dictionary<string, Font>
        {
            ["Droid Sans"] = new Font("https://fonts.googleapis.com/css?family=Droid+Sans:300,400,500,700"),
            ["Lato"] = new Font("https://fonts.googleapis.com/css?family=Lato:300,400,500,700"),
            ["Open Sans"] = new Font("https://fonts.googleapis.com/css?family=Open+Sans:300,400,500,700"),
            ["Roboto"] = new Font("https://fonts.googleapis.com/css?family=Roboto:300,400,500,700"),
            ["Ubuntu"] = new Font("https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700")
        };

        /// <summary>
        /// Gets the default xml entities.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> DefaultXmlEntities = new Dictionary<string, string>
        {
            ["&amp;"] = "&#38;",
            ["&apos;"] = "&#39;",
            ["&copy;"] = "&#169;",
            ["&gt;"] = "&#62;",
            ["&lt;"] = "&#60;",
            ["&nbsp;"] = "&#160;",
            ["&quot;"] = "&#34;",
            ["&reg;"] = "&#174;",
            ["&trade;"] = "&#8482;"
        };
        private IReadOnlyDictionary<string, string> xmlEntities = DefaultXmlEntities;

        static MjmlOptions()
        {
            DefaultParserContext = BuildContext(DefaultXmlEntities);
        }

        /// <summary>
        /// True to also keep comments. The default is: <c>false</c>.
        /// </summary>
        public bool KeepComments { get; init; }

        /// <summary>
        /// The default breakpoint to switch to mobile view.The default is: <c>"480px"</c>.
        /// </summary>
        public string Breakpoint { get; set; } = "480px";

        /// <summary>
        /// A list of custom styles. The default is: <c>false</c>.
        /// </summary>
        public Style[]? Styles { get; init; }

        /// <summary>
        /// True to enable media queries for OWA. The default is: <c>false</c>.
        /// </summary>
        public bool ForceOWAQueries { get; init; }

        /// <summary>
        /// True to beautify the HTML. The default is: <c>true</c>.
        /// </summary>
        public bool Beautify { get; init; } = true;

        /// <summary>
        /// True to minify the HTML. The default is: <c>false</c>.
        /// </summary>
        public bool Minify { get; init; }

        /// <summary>
        /// In lax mode some errors in the XML will be fixed. Only work when the MJML is passed in as string. The default is: <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Do not turn this on in production, because it can hurt performance.
        /// </remarks>
        public bool Lax { get; set; }

        /// <summary>
        /// The ID generator to create random values for attributes like Ids. The default is: <see cref="DefaultIDGenerator.Instance"/>.
        /// </summary>
        public IIdGenerator IdGenerator { get; init; } = DefaultIDGenerator.Instance;

        /// <summary>
        /// A list of supported default fonts. The default is: <see cref="DefaultFonts"/>.
        /// </summary>
        public IReadOnlyDictionary<string, Font> Fonts { get; init; } = DefaultFonts;

        /// <summary>
        /// The current validator. The default is: The default is: <c>null</c>.The default is: <c>null</c>.
        /// </summary>
        public IValidatorFactory? ValidatorFactory { get; init; }

        /// <summary>
        /// The file path loader for &lt;mj-include path="..." type="..."&gt; which handles loading the files from the specified path attribute. The default is: <c>null</c>.
        /// </summary>
        public IFileLoader? FileLoader { get; init; }

        /// <summary>
        /// The current parser context. Derived from xml entities.
        /// </summary>
        public XmlParserContext ParserContext { get; init; }

        /// <summary>
        /// A list of supported XML entities. The default is: <see cref="DefaultXmlEntities"/>.
        /// </summary>
        public IReadOnlyDictionary<string, string> XmlEntities
        {
            get => xmlEntities;
            init
            {
                xmlEntities = value;

                if (ReferenceEquals(value, DefaultXmlEntities))
                {
                    ParserContext = DefaultParserContext;
                }
                else
                {
                    ParserContext = BuildContext(value);
                }
            }
        }

        private static XmlParserContext BuildContext(IReadOnlyDictionary<string, string>? entities)
        {
            var context = new XmlParserContext(null, null, null, XmlSpace.None)
            {
                DocTypeName = "Html"
            };

            if (entities?.Count > 0)
            {
                var sb = DefaultPools.StringBuilders.Get();
                try
                {
                    foreach (var (key, value) in entities)
                    {
                        sb.AppendLine(CultureInfo.InvariantCulture, $"!ENTITY {key[1..]} \"{value}\">");
                    }

                    context.InternalSubset = sb.ToString();
                }
                finally
                {
                    DefaultPools.StringBuilders.Return(sb);
                }
            }

            return context;
        }
    }
}
