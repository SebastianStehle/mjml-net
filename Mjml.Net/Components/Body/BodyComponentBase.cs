namespace Mjml.Net.Components.Body
{
    public abstract class BodyComponentBase : Component
    {
#pragma warning disable RECS0108 // Warns about static fields in generic types
        private static readonly AllowedParents Parents = new AllowedParents
#pragma warning restore RECS0108 // Warns about static fields in generic types
        {
            "mj-body"
        };

        public override AllowedParents? AllowedAsDescendant => Parents;
    }
}
