using System.Xml;
using Mjml.Net.Helpers;

namespace Mjml.Net.Components
{
    public sealed partial class IncludeComponent : Component
    {
        public override string ComponentName => "mj-include";

        [Bind("path")]
        public string Path;

        [Bind("type")]
        public string Type;

        public bool IsMjml => string.Equals(Type, "mjml", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(Type);

        public override void AfterBind(GlobalContext context, XmlReader reader, IMjmlReader mjmlReader)
        {
            if (!IsMjml || string.IsNullOrWhiteSpace(Path))
            {
                return;
            }

            var content = context.Options.FileLoader?.LoadReader(Path);

            if (content != null)
            {
                mjmlReader.ReadFragment(content);
            }
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (IsMjml || string.IsNullOrWhiteSpace(Path))
            {
                RenderChildren(renderer, context);
                return;
            }

            var content = context.Options.FileLoader?.LoadText(Path);

            if (Type == "html" && content != null)
            {
                // Allow pretty formatting and indentation.
                renderer.Content(content);
                return;
            }

            if (Type == "css" && content != null)
            {
                // Allow multiple styles and render them later.
                context.SetGlobalData(content, Style.Static(content));
                return;
            }
        }
    }
}
