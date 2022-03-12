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
            var href = node.GetAttribute("href");

            // Just in case that validation is disabled.
            if (href != null)
            {
                // The name does not really matter.
                var name = node.GetAttribute("name");

                renderer.SetGlobalData(name ?? Guid.NewGuid().ToString(), new Font(href));
            }
        }
    }
}
