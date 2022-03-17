using System.Xml;

namespace Mjml.Net
{
    public abstract class Component<T> : IComponent where T : IProps, new()
    {
        private List<IComponent>? childNodes;
        private List<string>? childXml;

        protected IEnumerable<IComponent> ChildNodes
        {
            get => childNodes ?? Enumerable.Empty<IComponent>();
        }

        IProps IComponent.Props => Props;

        public INode Node { get; private set; }

        public abstract string Name { get; }

        public virtual AllowedParents? AllowedAsDescendant => null;

        public virtual AllowedParents? AllowedAsChild => null;

        public virtual ComponentType Type => ComponentType.Complex;

        public readonly T Props = new T();

        public abstract void Render(IHtmlRenderer renderer, GlobalContext context);

        public virtual void RenderChildren(IHtmlRenderer renderer, GlobalContext context)
        {
            if (childNodes == null)
            {
                return;
            }

            foreach (var child in childNodes)
            {
                child.Render(renderer, context);
            }
        }

        protected virtual void RenderRaw(IHtmlRenderer renderer)
        {
            if (childXml == null)
            {
                return;
            }

            foreach (var xml in childXml)
            {
                renderer.Content(xml);
            }
        }

        public virtual string? GetInheritingAttribute(string name)
        {
            return null;
        }

        public void AddChild(IComponent child)
        {
            childNodes ??= new List<IComponent>();
            childNodes.Add(child);
        }

        public void AddChild(string rawXml)
        {
            childXml ??= new List<string>();
            childXml.Add(rawXml);
        }

        public virtual void Bind(INode node, GlobalContext context, XmlReader reader)
        {
            Node = node;

            Props.Bind(node);
        }
    }
}
