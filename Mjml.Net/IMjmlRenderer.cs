namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        void Add(IComponent component);

        void Add(IHelper helper);

        string Render(string mjml, MjmlOptions options = default);

        string Render(Stream mjml, MjmlOptions options = default);

        string Render(TextReader mjml, MjmlOptions options = default);
    }
}
