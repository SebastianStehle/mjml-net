using System.Xml;
using Mjml.Net.Helpers;

namespace Mjml.Net.Components.Body
{
    public sealed partial class IncludeComponent : Component
    {
        private static readonly XmlReaderSettings ReaderSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

        public override string ComponentName => "mj-include";

        [Bind("path")]
        public string Path;

        [Bind("type")]
        public string Type;

        public bool IsMjml => string.Equals(Type, "mjml", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(Type);

        public override void AfterBind(GlobalContext context, XmlReader reader, IXmlReader xmlReader)
        {
            if (!IsMjml)
            {
                return;
            }

            var text = LoadText(context);

            if (text == null)
            {
                return;
            }

            using (var xml = XmlReader.Create(new StringReader(text), ReaderSettings))
            {
                xmlReader.ReadFragment(xml, this);
            }
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (IsMjml)
            {
                RenderChildren(renderer, context);
                return;
            }

            var text = LoadText(context);

            if (text == null)
            {
                return;
            }

            switch (Type)
            {
                case "html":
                    // Allow pretty formatting and indentation.
                    renderer.Content(text);
                    break;
                case "css":
                    // Allow multiple styles and render them later.
                    context.SetGlobalData(text, Style.Static(text));
                    break;
            }
        }

        private string? LoadText(GlobalContext context)
        {
            var fileLoader = context.Options.FileLoader;

            if (fileLoader == null || string.IsNullOrWhiteSpace(Path))
            {
                return null;
            }

            return fileLoader.LoadText(Path);
        }
    }
}
