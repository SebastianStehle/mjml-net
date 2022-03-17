using System.Xml;

namespace Mjml.Net
{
    public interface IComponent
    {
        AllowedParents? AllowedAsDescendant { get; }

        AllowedParents? AllowedAsChild { get; }

        IProps Props { get; }

        INode Node { get; }

        ComponentType Type { get; }

        string? GetInheritingAttribute(string name);

        void Bind(INode node, GlobalContext context, XmlReader reader);

        void AddChild(IComponent child);

        void AddChild(string rawXml);

        void Render(IHtmlRenderer renderer, GlobalContext context);
    }
}
