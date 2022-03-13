namespace Mjml.Net.Components.Body
{
    public abstract class BodyComponentBase : IComponent
    {
        public virtual AllowedParents? AllowedParents { get; } =
            new AllowedParents
            {
                "mj-body"
            };

        public abstract string ComponentName { get; }

        public abstract void Render(IHtmlRenderer renderer, INode node);
    }
}
