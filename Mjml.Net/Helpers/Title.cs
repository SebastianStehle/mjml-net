namespace Mjml.Net.Helpers
{
    public sealed record Title(string Value)
    {
    }

    public sealed class TitleHelper : IHelper
    {
        public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalData data)
        {
            if (target != HelperTarget.HeadStart)
            {
                return;
            }

            var title = data.Values.OfType<Title>().FirstOrDefault()?.Value;

            renderer.ElementStart("title");
            renderer.Content(title);
            renderer.ElementEnd("title");
        }
    }
}
