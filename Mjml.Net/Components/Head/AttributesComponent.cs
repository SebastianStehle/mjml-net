
using HtmlPerformanceKit;

namespace Mjml.Net.Components.Head
{
    public partial class AttributesComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-attributes";

        public override void Bind(IBinder binder, GlobalContext context, IHtmlReader reader)
        {
            base.Bind(binder, context, reader);

            while (reader.Read())
            {
                switch (reader.TokenKind)
                {
                    case HtmlTokenKind.EndTag:
                        return;
                    case HtmlTokenKind.Tag when reader.Name == "mj-class":
                        var className = reader.GetAttribute("name");

                        if (className != null)
                        {
                            for (var i = 0; i < reader.AttributeCount; i++)
                            {
                                var attributeName = reader.GetAttributeName(i);
                                var attributeValue = reader.GetAttribute(i);

                                if (attributeName != "name")
                                {
                                    context.SetClassAttribute(attributeName, className, attributeValue);
                                }
                            }
                        }

                        reader.Read();
                        break;

                    case HtmlTokenKind.Tag:
                        var tagName = reader.Name;

                        for (var i = 0; i < reader.AttributeCount; i++)
                        {
                            var attributeName = reader.GetAttributeName(i);
                            var attributeValue = reader.GetAttribute(i);

                            context.SetTypeAttribute(attributeName, tagName, attributeValue);
                        }

                        reader.Read();
                        break;
                }
            }
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
        }
    }
}
