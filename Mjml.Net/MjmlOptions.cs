namespace Mjml.Net
{
    public record struct MjmlOptions
    {
        public bool KeepComments { get; set; }

        public string[]? Fonts { get; set; }

        public bool Beautify { get; set; } = true;

        public bool Minify { get; set; }

        public IValidatorFactory? ValidatorFactory { get; set; }
    }
}
