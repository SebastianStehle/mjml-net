namespace Mjml.Net.Helpers
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record Title(string Value)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
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
