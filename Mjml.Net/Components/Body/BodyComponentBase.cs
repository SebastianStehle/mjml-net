namespace Mjml.Net.Components.Body
{
    public abstract class BodyComponentBase : Component
    {
        private static readonly AllowedParents Parents = new AllowedParents
        {
            "mj-body"
        };

        public override AllowedParents? AllowedAsDescendant => Parents;
    }
}
