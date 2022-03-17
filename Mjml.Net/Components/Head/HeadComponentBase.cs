namespace Mjml.Net.Components.Head
{
    public abstract class HeadComponentBase : Component
    {
#pragma warning disable RECS0108 // Warns about static fields in generic types
        private static readonly AllowedParents? Parents = new AllowedParents
#pragma warning restore RECS0108 // Warns about static fields in generic types
        {
            "mj-head"
        };

        public override AllowedParents? AllowedAsDescendant => Parents;
    }
}
