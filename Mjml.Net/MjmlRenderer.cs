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

            return Render(xml, options);
        }

        public string Render(Stream mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        public string Render(TextReader mjml, MjmlOptions options = default)
        {
            var xml = XmlReader.Create(mjml);

            return Render(xml, options);
        }

        private string Render(XmlReader xml, MjmlOptions options)
        {
            var context = ObjectPools.Contexts.Get();
            try
            {
                context.Setup(this, xml, options);
                context.Read();

                return context.ToString()!;
            }
            finally
            {
                ObjectPools.Contexts.Return(context);
            }
        }
    }
}
