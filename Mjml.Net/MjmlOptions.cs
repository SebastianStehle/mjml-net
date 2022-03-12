namespace Mjml.Net
{
    public record struct MjmlOptions
    {
        public bool KeepComments { get; set; }

        public string[] Fonts { get; set; }

        public bool Beautify { get; set; }

        public bool Minify { get; set; }
    }
}
