namespace Mjml.Net.Components
{
    public sealed class CommentComponent : Component
    {
        public override string ComponentName => "comment";

        public override bool Raw => true;

        public string Text;

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            renderer.Content($"<!-- {Text} -->");
        }
    }
}
