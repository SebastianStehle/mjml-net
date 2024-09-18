using AngleSharp.Dom;

namespace Mjml.Net;

public sealed class AttributesPostProcessor : IAngleSharpPostProcessor
{
    public static readonly IPostProcessor Instance = new AngleSharpPostProcessor(new AttributesPostProcessor());

    public ValueTask ProcessAsync(IDocument document, MjmlOptions options, CancellationToken ct)
    {
        foreach (var attributes in document.QuerySelectorAll("mj-html-attributes"))
        {
            foreach (var selector in attributes.Children("mj-selector"))
            {
                var path = selector.GetAttribute("path");

                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                var attributeValues = selector.Children("mj-html-attribute")
                    .Select(x =>
                    {
                        var attributeName = x.GetAttribute("name")!;
                        var attributeValue = x.TextContent;

                        return (Name: attributeName, Value: attributeValue);
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .ToList();

                foreach (var target in document.QuerySelectorAll(path))
                {
                    foreach (var (name, value) in attributeValues)
                    {
                        target.SetAttribute(name, value?.Trim());
                    }
                }
            }
        }

        RemoveAll(document, "mj-html-attributes");
        RemoveAll(document, "mj-html-attribute");
        RemoveAll(document, "mj-selector");

        return default;
    }

    private static void RemoveAll(IDocument document, string selector)
    {
        foreach (var element in document.QuerySelectorAll(selector))
        {
            element.Remove();
        }
    }
}
