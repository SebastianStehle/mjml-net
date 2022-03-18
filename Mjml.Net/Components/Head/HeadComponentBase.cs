namespace Mjml.Net.Components.Head
{
    public abstract class HeadComponentBase : Component
    {
        private static readonly AllowedParents? Parents = new AllowedParents
        {
            "mj-head"
        };

        public override AllowedParents? AllowedAsDescendant => Parents;
    }
}
