namespace Mjml.Net
{
    public interface IComponent
    {
        public AllowedParents? AllowedAsDescendant => null;

        public AllowedParents? AllowedAsChild => null;

        public IProps? Props => null;

        public bool SelfClosed => false;

        public bool Raw => false;

        public bool NeedsContent => false;

        public string ComponentName { get; }

        public void Render(IHtmlRenderer renderer, INode node);
    }
}
