namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        IMjmlRenderer Add(IComponent component);

        IMjmlRenderer Add(IHelper helper);

        IMjmlRenderer ClearComponents();

        IMjmlRenderer ClearHelpers();

        string Render(string mjml, MjmlOptions options = default);

        string Render(Stream mjml, MjmlOptions options = default);

        string Render(TextReader mjml, MjmlOptions options = default);
    }
}
