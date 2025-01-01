using HtmlPerformanceKit;

namespace Mjml.Net.Components.Head;

public partial class AttributesComponent : HeadComponentBase
{
    public override string ComponentName => "mj-attributes";

    public override void Read(IHtmlReader htmlReader, IMjmlReader mjmlReader, GlobalContext context)
    {
        while (htmlReader.Read())
        {
            switch (htmlReader.TokenKind)
            {
                case HtmlTokenKind.EndTag:
                    return;
                case HtmlTokenKind.Tag when htmlReader.Name == "mj-class":
                    var className = htmlReader.GetAttribute("name");
                    if (string.IsNullOrWhiteSpace(className))
                    {
                        htmlReader.Read();
                        break;
                    }

                    for (var i = 0; i < htmlReader.AttributeCount; i++)
                    {
                        var attributeName = htmlReader.GetAttributeName(i);
                        var attributeValue = htmlReader.GetAttribute(i);

                        if (attributeName != "name")
                        {
                            context.SetClassAttribute(attributeName, className, attributeValue);
                        }
                    }

                    if (htmlReader.SelfClosingElement)
                    {
                        htmlReader.Read();
                        break;
                    }

                    var subTree = htmlReader.ReadSubtree();
                    while (subTree.Read())
                    {
                        if (subTree.TokenKind == HtmlTokenKind.Tag)
                        {
                            var childTagName = htmlReader.Name;

                            for (var i = 0; i < subTree.AttributeCount; i++)
                            {
                                var attributeName = subTree.GetAttributeName(i);
                                var attributeValue = subTree.GetAttribute(i);

                                context.SetParentClassAttribute(attributeName, className, childTagName, attributeValue);
                            }
                        }
                    }
                    break;

                case HtmlTokenKind.Tag:
                    var tagName = htmlReader.Name;

                    for (var i = 0; i < htmlReader.AttributeCount; i++)
                    {
                        var attributeName = htmlReader.GetAttributeName(i);
                        var attributeValue = htmlReader.GetAttribute(i);

                        context.SetTypeAttribute(attributeName, tagName, attributeValue);
                    }

                    htmlReader.Read();
                    break;
            }
        }
    }

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
    }
}
