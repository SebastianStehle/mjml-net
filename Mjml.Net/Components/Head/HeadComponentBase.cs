namespace Mjml.Net.Components.Head
{
    public abstract class HeadComponentBase : IComponent
    {
        public virtual AllowedParents? AllowedAsChild => null;

        public virtual AllowedParents? AllowedAsDescendant { get; } =
            new AllowedParents
            {
                "mj-head"
            };

        public virtual IProps? Props => null;

        public virtual bool SelfClosed => false;

        public virtual bool NeedsContent => false;

        public abstract string ComponentName { get; }

        public abstract void Render(IHtmlRenderer renderer, INode node);
    }

    public abstract class HeadComponentBase<T> : HeadComponentBase where T : struct, IProps
    {
        public override IProps Props { get; } = default(T);
    }
}
