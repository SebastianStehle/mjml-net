namespace Mjml.Net
{
    public struct ValidationContext
    {
        public MjmlOptions Options { get; set; }

        public int? XmlColumn { get; set; }

        public int? XmlLine { get; set; }
    }
}
