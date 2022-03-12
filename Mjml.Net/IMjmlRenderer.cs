namespace Mjml.Net
{
    public interface IMjmlRenderer
    {
        void Add(IComponent component);

        void Add(IHelper helper);

        string Render(string mjml);

        string Render(Stream mjml);

        string Render(TextReader mjml);
    }
}
