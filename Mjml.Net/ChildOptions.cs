namespace Mjml.Net
{
    public struct ChildOptions<T>
    {
        public bool RawXML { get; set; }

        /// <summary>
        /// Define a context value to be passed to the renderer. This context can be used to avoid extra allocations.
        /// </summary>
        public T Context { get; set; }

        public Action<T, IChildRenderer, IHtmlRenderer, INode>? Renderer { get; set; }
    }
}
