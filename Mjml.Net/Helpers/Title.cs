﻿namespace Mjml.Net.Helpers;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public sealed record Title(InnerTextOrHtml Value) : GlobalData;
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

public sealed class TitleHelper : IHelper
{
    public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalContext context)
    {
        if (target != HelperTarget.HeadStart)
        {
            return;
        }

        var title = context.GlobalData.Values.OfType<Title>().FirstOrDefault()?.Value;

        if (title == null)
        {
            return;
        }

        renderer.StartElement("title");
        renderer.Content(title);
        renderer.EndElement("title");
    }
}
