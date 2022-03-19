using System.Xml;

namespace Mjml.Net
{
    public interface IComponent
    {
        AllowedParents? AllowedAsDescendant { get; }

        AllowedParents? AllowedAsChild { get; }

        AllowedAttributes? AllowedFields { get; }

        IEnumerable<IComponent> ChildNodes { get; }

        INode Node { get; }

        ContentType ContentType { get; }

        bool Raw { get; }

        string ComponentName { get; }

        string? GetDefaultValue(string name);

        string? GetInheritingAttribute(string name);

        int ActualWidth { get; }

        void Bind(INode node, GlobalContext context, XmlReader reader);

        void AddChild(IComponent child);

        void AddChild(string rawXml);

        void Render(IHtmlRenderer renderer, GlobalContext context);

        void Measure(int parentWidth, int numSiblings, int numNonRawSiblings);
    }
}
