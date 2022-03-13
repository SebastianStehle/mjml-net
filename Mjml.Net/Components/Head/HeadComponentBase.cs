namespace Mjml.Net.Components.Head
{
    public abstract class HeadComponentBase : IComponent
    {
        public virtual AllowedParents? AllowedParents { get; } =
            new AllowedParents
            {
                "mj-head"
            };

        public virtual AllowedAttributes? AllowedAttributes => null;

        public virtual Attributes? DefaultAttributes => null;

        public virtual bool SelfClosed => false;

        public virtual bool NeedsContent => false;

        public abstract string ComponentName { get; }

        public abstract void Render(IHtmlRenderer renderer, INode node);
    }
}
