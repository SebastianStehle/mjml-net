using System.Xml;

namespace Mjml.Net
{
    public sealed class MjmlRenderer : IMjmlRenderer
    {
        private readonly Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();
        private readonly List<IHelper> helpers = new List<IHelper>();

        public IEnumerable<IHelper> Helpers => helpers;

        public void Add(IComponent component)
        {
            components[component.ComponentName] = component;
        }

        public void Add(IHelper helper)
        {
            helpers.Add(helper);
        }

        internal IComponent GetComponent(string previousElement)
        {
            return components[previousElement];
        }

        public string Render(string mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(new StringReader(mjml));

            return Render(xml);
        }

        public string Render(Stream mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml);
        }

        public string Render(TextReader mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml);
        }

        private string Render(XmlReader xml)
        {
            var context = new MjmlRenderContext(this, xml);

            context.Read();

            return context.ToString()!;
        }
    }
}
