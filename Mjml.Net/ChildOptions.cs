namespace Mjml.Net
{
    public struct ChildOptions
    {
        public bool RawXML { get; set; }

        public Action<IChildRenderer>? Renderer { get; set; }

        public Action<IContext>? ChildContext { get; set; }
    }
}
