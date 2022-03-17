namespace Mjml.Net
{
    public struct ChildOptions
    {
        public Action<IChildRenderer>? Renderer { get; set; }

        public Action<IContext>? ChildContext { get; set; }
    }
}
