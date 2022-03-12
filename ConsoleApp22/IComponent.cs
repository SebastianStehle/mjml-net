namespace ConsoleApp22
{
    public interface IComponent
    {
        public AllowedAttributes? AllowedAttributes => null;

        public Attributes? DefaultAttributes => null;

        public string ComponentName { get; }

        public void Render(IHtmlRenderer renderer, INode node);
    }
}
