namespace ConsoleApp22.Components
{
    public sealed class BodyComponent : IComponent
    {
        public string ComponentName => "mj-body";

        public AllowedAttributes? AllowedAttributes { get; } =
            new AllowedAttributes
            {
                ["width"] = AttributeType.Pixels,
                ["background-color"] = AttributeType.Color
            };

        public Attributes? DefaultAttributes { get; } =
            new Attributes
            {
                ["width"] = "600px"
            };

        public void Render(IHtmlRenderer renderer, INode node)
        {
            renderer.SetContext("width", node.GetAttribute("width"));

            renderer.StartElement("body")
                .Style("word-spacing", "normal")
                .Done();

            renderer.StartElement("div")
                .Attr("class", node.GetAttribute("css-class"))
                .Style("background-color", node.GetAttribute("background-color"))
                .Done();

            renderer.RenderChildren();

            renderer.EndElement("div");
            renderer.EndElement("body");
        }
    }
}
