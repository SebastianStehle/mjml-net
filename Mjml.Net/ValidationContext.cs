namespace Mjml.Net
{
    public struct ValidationContext
    {
        public MjmlOptions Options { get; set; }

        public int? LinePosition { get; set; }

        public int? LineNumber { get; set; }
    }
}
