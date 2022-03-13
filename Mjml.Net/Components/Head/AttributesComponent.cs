using System.Xml;

namespace Mjml.Net.Components.Head
{
    public sealed class AttributesComponent : HeadComponentBase
    {
        public override string ComponentName => "mj-attributes";

        public override void Render(IHtmlRenderer renderer, INode node)
        {
            var reader = node.Reader;

            using var subtree = reader.ReadSubtree();

            while (subtree.Read())
            {
                if (subtree.NodeType == XmlNodeType.Element)
                {
                    var type = subtree.Name;

                    if (type == "mj-class")
                    {
                        if (reader.MoveToAttribute("name"))
                        {
                            var className = subtree.Value;

                            for (var i = 0; i < subtree.AttributeCount; i++)
                            {
                                subtree.MoveToAttribute(i);

                                if (subtree.Name != "name")
                                {
                                    renderer.SetClassAttribute(subtree.Name, className, subtree.Value);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < subtree.AttributeCount; i++)
                        {
                            subtree.MoveToAttribute(i);

                            renderer.SetTypeAttribute(subtree.Name, type, subtree.Value);
                        }
                    }
                }
            }

            subtree.Close();
        }
    }
}
