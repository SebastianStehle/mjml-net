using System.Xml;

namespace Mjml.Net.Components.Head
{
    public partial class AttributesComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-attributes";

        public override void Bind(INode node, GlobalContext context, XmlReader reader)
        {
            base.Bind(node, context, reader);

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    var type = reader.Name;

                    if (type == "mj-class")
                    {
                        if (reader.MoveToAttribute("name"))
                        {
                            var className = reader.Value;

                            for (var i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);

                                if (reader.Name != "name")
                                {
                                    context.SetClassAttribute(reader.Name, className, reader.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);

                            context.SetTypeAttribute(reader.Name, type, reader.Value);
                        }
                    }
                }
            }
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
        }
    }
}
