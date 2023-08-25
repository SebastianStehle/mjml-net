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

                    if (className != null)
                    {
                        for (var i = 0; i < htmlReader.AttributeCount; i++)
                        {
                            var attributeName = htmlReader.GetAttributeName(i);
                            var attributeValue = htmlReader.GetAttribute(i);

                            if (attributeName != "name")
                            {
                                context.SetClassAttribute(attributeName, className, attributeValue);
                            }
                        }
                    }

                    htmlReader.Read();
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
