using Mjml.Net.Helpers;

namespace Mjml.Net
{
    public sealed record MjmlOptions
    {
        private static readonly Dictionary<string, Font> DefaultFonts = new Dictionary<string, Font>
        {
            ["Droid Sans"] = new Font("https://fonts.googleapis.com/css?family=Droid+Sans:300,400,500,700"),
            ["Lato"] = new Font("https://fonts.googleapis.com/css?family=Lato:300,400,500,700"),
            ["Open Sans"] = new Font("https://fonts.googleapis.com/css?family=Open+Sans:300,400,500,700"),
            ["Roboto"] = new Font("https://fonts.googleapis.com/css?family=Roboto:300,400,500,700"),
            ["Ubuntu"] = new Font("https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700")
        };

        private static readonly Dictionary<string, string> DefaultXmlEntities = new Dictionary<string, string>
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

        /// <summary>
        /// True to also keep comments.
        /// </summary>
        public bool KeepComments { get; init; } = false;

        /// <summary>
        /// The default breakpoint to switch to mobile view.
        /// </summary>
        public string Breakpoint { get; set; } = "480px";

        /// <summary>
        /// A list of custom styles.
        /// </summary>
        public Style[]? Styles { get; init; }

        /// <summary>
        /// True to enable media queries for OWA.
        /// </summary>
        public bool ForceOWAQueries { get; init; }

        /// <summary>
        /// True to beatify the HTML.
        /// </summary>
        public bool Beautify { get; init; } = true;

        /// <summary>
        /// True to minify the HTML.
        /// </summary>
        public bool Minify { get; init; }

        /// <summary>
        /// In lax mode some errors in the XML will be fixed. Only work when the MJML is passed in as string.
        /// </summary>
        /// <remarks>
        /// Do not turn this on in production, because it can hurt performance.
        /// </remarks>
        public bool Lax { get; set; }

        /// <summary>
        /// The ID generator to create random values for attributes like Ids.
        /// </summary>
        public IIdGenerator IdGenerator { get; init; } = DefaultIDGenerator.Instance;

        /// <summary>
        /// A list of supported default fonts.
        /// </summary>
        public IReadOnlyDictionary<string, Font> Fonts { get; init; } = DefaultFonts;

        /// <summary>
        /// A list of supported XML entities.
        /// </summary>
        public IReadOnlyDictionary<string, string> XmlEntities { get; init; } = DefaultXmlEntities;

        /// <summary>
        /// The current validator.
        /// </summary>
        public IValidatorFactory? ValidatorFactory { get; init; }
    }
}
