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

        public bool KeepComments { get; init; }

        public string Breakpoint { get; set; } = "480px";

        public bool ForceOWAQueries { get; init; }

        public bool Beautify { get; init; } = true;

        public bool Minify { get; init; }

        public IReadOnlyDictionary<string, Font> Fonts { get; init; } = DefaultFonts;

        public IValidatorFactory? ValidatorFactory { get; init; }
    }
}
