namespace Mjml.Net.Components.Body
{
    public abstract class BodyComponentBase : IComponent
    {
        public virtual AllowedParents? AllowedParents { get; } =
            new AllowedParents
            {
                "mj-body"
            };

        public virtual IProps? Props => null;

        public virtual bool SelfClosed => false;

        public virtual bool Raw => false;

        public virtual bool NeedsContent => false;

        public abstract string ComponentName { get; }

        public abstract void Render(IHtmlRenderer renderer, INode node);
    }

    public abstract class BodyComponentBase<T> : BodyComponentBase where T : struct, IProps
    {
        public override IProps Props { get; } = default(T);
    }
}
