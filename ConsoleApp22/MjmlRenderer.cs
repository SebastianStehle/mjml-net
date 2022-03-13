using System.Xml;

namespace ConsoleApp22
{
    public sealed class MjmlRenderer
    {
        private readonly Dictionary<string, IComponent> components = new Dictionary<string, IComponent>();

        public void Add(IComponent component)
        {
            components[component.ComponentName] = component;
        }

        public string Render(string mjml)
        {
            var xml = XmlReader.Create(new StringReader(mjml));

            var context = new MjmlRenderContext(this, xml);

            context.Read();

            return context.ToString()!;
        }

        internal IComponent GetComponent(string previousElement)
        {
            return components[previousElement];
        }
    }
}
