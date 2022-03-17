namespace Mjml.Net
{
    public struct ChildOptions
    {
        public Action<IContext>? ChildContext { get; set; }

        public Func<string, string?>? ChildResolver { get; set; }
    }
}
