namespace Mjml.Net.Components.Body
{
    public abstract partial class BodyComponentBase : Component
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-body"
        };

        public override AllowedParents? AllowedAsDescendant => Parents;

        [Bind("css-class")]
        public string? CssClass;
    }
}
