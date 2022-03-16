namespace Mjml.Net
{
    public interface IComponent
    {
        public AllowedParents? AllowedParents { get; }

        public IProps? Props => null;

        public bool SelfClosed => false;

        public bool Raw => false;

        public bool NeedsContent => false;

        public string ComponentName { get; }

        public void Render(IHtmlRenderer renderer, INode node);
    }
}
