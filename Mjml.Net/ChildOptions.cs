namespace Mjml.Net
{
    public struct ChildOptions<T>
    {
        public bool RawXML { get; set; }

        public T Context { get; set; }

        public Action<T, IChildRenderer, IHtmlRenderer, INode>? Renderer { get; set; }
    }
}
