namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        IMjmlRenderer Add<T>(string name) where T : IComponent, new();

        IMjmlRenderer Add(IHelper helper);

        IMjmlRenderer ClearComponents();

        IMjmlRenderer ClearHelpers();

        RenderResult Render(string mjml, MjmlOptions options = default);

        RenderResult Render(Stream mjml, MjmlOptions options = default);

        RenderResult Render(TextReader mjml, MjmlOptions options = default);
    }
}
