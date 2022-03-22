using System.Xml;
using Mjml.Net.Extensions;

namespace Mjml.Net
{
    public abstract class Component : IComponent
    {
        private List<IComponent>? childNodes;
        private List<string>? childXml;

        public IEnumerable<IComponent> ChildNodes
        {
            get => childNodes ?? Enumerable.Empty<IComponent>();
        }

        public double ActualWidth { get; protected set; }

        public virtual bool Raw => false;

        public virtual AllowedParents? AllowedAsDescendant => null;

        public virtual AllowedParents? AllowedAsChild => null;

        public virtual AllowedAttributes? AllowedFields => null;

        public virtual ContentType ContentType => ContentType.Complex;

        public abstract string ComponentName { get; }

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
            if (childXml == null || childXml.Count == 0)
            {
                return;
            }

            var i = 0;
            foreach (var xml in childXml)
            {
                var toRender = xml.AsSpan();

                if (i == 0)
                {
                    toRender = toRender.TrimXmlStart();
                }

                if (i == childXml.Count - 1)
                {
                    toRender = toRender.TrimXmlEnd();
                }

                renderer.Plain(toRender);
            }
        }

        public virtual string? GetInheritingAttribute(string name)
        {
            return null;
        }

        public virtual string? GetDefaultValue(string name)
        {
            return null;
        }

        public virtual string? GetAttribute(string name)
        {
            return null;
        }

        public void AddChild(string rawXml)
        {
            childXml ??= new List<string>(1);
            childXml.Add(rawXml);
        }

        public void AddChild(IComponent child)
        {
            childNodes ??= new List<IComponent>(1);
            childNodes.Add(child);
        }

        public virtual void Bind(IBinder binder, GlobalContext context, XmlReader reader)
        {
        }

        public virtual void Measure(double parentWidth, int numSiblings, int numNonRawSiblings)
        {
            ActualWidth = parentWidth;

            MeasureChildren(ActualWidth);
        }

        protected void MeasureChildren(double width)
        {
            if (childNodes != null)
            {
                foreach (var child in childNodes)
                {
                    child.Measure(width, childNodes.Count, childNodes.Count(x => !x.Raw));
                }
            }
        }
    }
}
