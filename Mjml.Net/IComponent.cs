namespace Mjml.Net
{
    public interface IComponent
    {
        public AllowedAttributes? AllowedAttributes => null;

        public Attributes? DefaultAttributes => null;

        public bool SelfClosed => false;

        public string ComponentName { get; }

        public void Render(IHtmlRenderer renderer, INode node);
    }
}
