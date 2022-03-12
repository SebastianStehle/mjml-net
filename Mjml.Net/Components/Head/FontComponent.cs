using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Head
{
    public sealed class FontComponent : IComponent
    {
        public string ComponentName => "mj-font";

        public bool SelfClosed => true;

        public AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["name"] = AttributeType.String,
                ["href"] = AttributeType.String
            };

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.SetGlobalData("font", new Font(node.GetAttribute("name"), node.GetAttribute("href")));
        }
    }
}
