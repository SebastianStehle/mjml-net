using System.Xml;
using Mjml.Net.Helpers;
using Mjml.Net.Types;

namespace Mjml.Net.Components
{
    public sealed partial class IncludeComponent : Component
    {
        public override string ComponentName => "mj-include";

        public sealed class PathValidator : IType
        {
            public bool Validate(string value, ref ValidationContext context)
            {
                return !string.IsNullOrWhiteSpace(value) && context.Options?.FileLoader?.ContainsFile(value) == true;
            }
        }

        internal sealed class TypeValidator : EnumType
        {
            public TypeValidator()
                : base(false, "mjml", "html", "css")
            {
            }
        }

        [Bind("path", typeof(PathValidator))]
        public string Path;

        [Bind("type", typeof(TypeValidator))]
        public string Type;

        public IncludeType ActualType { get; private set; }

        public override void AfterBind(GlobalContext context, XmlReader reader, IMjmlReader mjmlReader)
        {
            switch (Type)
            {
                case "html":
                    ActualType = IncludeType.Html;
                    break;
                case "css":
                    ActualType = IncludeType.Css;
                    break;
                case "mjml":
                    ActualType = IncludeType.Mjml;
                    break;
                default:
                    if (Path?.EndsWith(".html", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ActualType = IncludeType.Html;
                    }
                    else if (Path?.EndsWith(".css", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        ActualType = IncludeType.Css;
                    }
                    else
                    {
                        ActualType = IncludeType.Mjml;
                    }

                    break;
            }

            if (ActualType != IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
            {
                return;
            }

            var content = context.Options.FileLoader?.LoadReader(Path);

            if (content == null)
            {
                return;
            }

            mjmlReader.ReadFragment(content);
        }

        public override void Render(IHtmlRenderer renderer, GlobalContext context)
        {
            if (ActualType == IncludeType.Mjml || string.IsNullOrWhiteSpace(Path))
            {
                // Render the children that have been added in the bin method.
                RenderChildren(renderer, context);
                return;
            }

            var content = context.Options.FileLoader?.LoadText(Path);

            if (content == null)
            {
                return;
            }

            if (ActualType == IncludeType.Html)
            {
                // Allow pretty formatting and indentation.
                renderer.Content(content);
            }
            else if (ActualType == IncludeType.Css)
            {
                // Allow multiple styles and render them later.
                context.SetGlobalData(content, Style.Static(content));
            }
        }
    }
}
