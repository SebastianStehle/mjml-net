namespace Mjml.Net
{
    public sealed record MjmlOptions
    {
        public bool KeepComments { get; init; }

        public string[]? Fonts { get; init; }

        public string Breakpoint { get; set; } = "480px";

        public bool ForceOWAQueries { get; init; }

        public bool Beautify { get; init; } = true;

        public bool Minify { get; init; }

        public IValidatorFactory? ValidatorFactory { get; init; }
    }
}
