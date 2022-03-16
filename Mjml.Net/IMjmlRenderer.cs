namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        IMjmlRenderer Add(IComponent component);

        IMjmlRenderer Add(IHelper helper);

        IMjmlRenderer ClearComponents();

        IMjmlRenderer ClearHelpers();

        RenderResult Render(string mjml, MjmlOptions options = default);

        RenderResult Render(Stream mjml, MjmlOptions options = default);
    }
}
