namespace Mjml.Net
{
    public struct ChildOptions
    {
        public bool RawXML { get; set; }

        public Action<IChildRenderer>? Renderer { get; set; }

        public Dictionary<string, object?> Values { get; set; }
    }
}
