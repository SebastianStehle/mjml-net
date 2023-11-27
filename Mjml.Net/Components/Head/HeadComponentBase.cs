namespace Mjml.Net.Components.Head;

public abstract class HeadComponentBase : Component
{
    private static readonly AllowedParents? Parents =
    [
        "mj-head"
    ];

    public override AllowedParents? AllowedParents => Parents;
}
