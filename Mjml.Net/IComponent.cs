namespace Mjml.Net
{
    public interface IComponent
    {
        public AllowedParents? AllowedParents { get; }

        public AllowedAttributes? AllowedAttributes => null;

        public Attributes? DefaultAttributes => null;

        public bool SelfClosed => false;

        public bool Raw => false;

        public bool NeedsContent => false;

        public string ComponentName { get; }

        public void AddToChildContext(IContext context, IContext parentContext, INode parentNode)
        {
        }

        public void Render(IHtmlRenderer renderer, INode node);
    }
}
