namespace ConsoleApp22.Components.Head
{
    internal class FontComponent : IComponent
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

            renderer.Style(href);
            renderer.InlineStyle($"@import url({href});");
        }
    }
}
