﻿namespace Mjml.Net.Components.Head;

public sealed class HeadComponent : Component
{
    private static readonly AllowedParents Parents = new AllowedParents
    {
        "mjml"
    };

    public override AllowedParents? AllowedParents => Parents;

    public override string ComponentName => "mj-head";

    public override void Render(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartBuffer();

        RenderChildren(renderer, context);

        context.AddGlobalData(new HeadBuffer(renderer.EndBuffer()));
    }
}
