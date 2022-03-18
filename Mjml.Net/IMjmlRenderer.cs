namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        IMjmlRenderer Add<T>() where T : IComponent, new();

        IMjmlRenderer Add(IHelper helper);

        IMjmlRenderer ClearComponents();

        IMjmlRenderer ClearHelpers();

        RenderResult Render(string mjml, MjmlOptions? options = null);

        RenderResult Render(Stream mjml, MjmlOptions? options = null);

        RenderResult Render(TextReader mjml, MjmlOptions? options = null);
    }
}
