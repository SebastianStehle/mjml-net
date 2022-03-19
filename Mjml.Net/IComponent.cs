using System.Xml;

namespace Mjml.Net
{
    public interface IComponent
    {
        AllowedParents? AllowedAsDescendant { get; }

        AllowedParents? AllowedAsChild { get; }

        AllowedAttributes? AllowedFields { get; }

        IEnumerable<IComponent> ChildNodes { get; }

        ContentType ContentType { get; }

        bool Raw { get; }

        string ComponentName { get; }

        string? GetDefaultValue(string name);

        string? GetInheritingAttribute(string name);

        string? GetAttribute(string name);

        void Bind(IBinder node, GlobalContext context, XmlReader reader);

        void AddChild(IComponent child);

        void AddChild(string rawXml);

        void Render(IHtmlRenderer renderer, GlobalContext context);
    }
}
