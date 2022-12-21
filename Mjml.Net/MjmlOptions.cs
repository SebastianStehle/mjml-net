using Mjml.Net.Helpers;

namespace Mjml.Net
{
    /// <summary>
    /// Provides options to configure the MJML rendering process.
    /// </summary>
    public sealed record MjmlOptions
    {
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
        /// A list of supported XML entities.
        /// </summary>
        [Obsolete("Not needed anymore.")]
        public IReadOnlyDictionary<string, string>? XmlEntities { get; set; } = null!;
    }
}
