using AngleSharp.Dom;

namespace Mjml.Net;

public static class AngleSharpExtensions
{
    public static void Traverse(this INode node, Action<IElement> action)
    {
        foreach (var child in node.ChildNodes.ToList())
        {
            Traverse(child, action);
        }

        if (node is IElement element)
        {
            action(element);
        }
    }

    public static IEnumerable<IElement> Children(this IElement node, string tagName)
    {
        return node.Children.Where(x => string.Equals(x.NodeName, tagName, StringComparison.OrdinalIgnoreCase));
    }
}
