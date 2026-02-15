#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Helpers;

public sealed record Style(Action<IHtmlRenderer, GlobalContext> Renderer, bool Inline = false) : GlobalData
{
    public static Style Static(InnerTextOrHtml text, bool inline)
    {
        return new Style((renderer, _) => renderer.Content(text), inline);
    }
}

public sealed record MediaQuery(string ClassName, string Rule) : GlobalData
{
    public static MediaQuery Width(string className, string width)
    {
        return new MediaQuery(className, FormattableString.Invariant($"{{\r\nwidth:{width} !important;\r\nmax-width: {width};\r\n}}"));
    }
}

public sealed class StyleHelper : IHelper
{
    public void Render(IHtmlRenderer renderer, HelperTarget target, GlobalContext context)
    {
        if (target != HelperTarget.HeadEnd)
        {
            return;
        }

        WriteMediaQueries(renderer, context);
        WriteMediaQueriesThunderbird(renderer, context);
        WriteMediaQueriesOWA(renderer, context);
        WriteOptionStyles(renderer, context);
        WriteStyles(renderer, context);
    }

    private static void WriteMediaQueries(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("style")
            .Attr("type", "text/css");

        renderer.Content($"@media only screen and (min-width:{context.Options.Breakpoint}) {{");

        foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
        {
            renderer.Content($"  .{mediaQuery.ClassName} {mediaQuery.Rule}");
        }

        renderer.Content("}");

        renderer.EndElement("style");
    }

    private static void WriteMediaQueriesThunderbird(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("style")
            .Attr("media", $"screen and (min-width:{context.Options.Breakpoint})");

        foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
        {
            renderer.Content($"  .moz-text-html .{mediaQuery.ClassName} {mediaQuery.Rule}");
        }

        renderer.EndElement("style");
    }

    private static void WriteStyles(IHtmlRenderer renderer, GlobalContext context)
    {
        List<Style>? regularStyles = null;
        List<Style>? inlineStyles = null;

        // Separate inline and non-inline styles in a single pass
        foreach (var item in context.GlobalData.Values)
        {
            if (item is Style style)
            {
                if (style.Inline)
                {
                    inlineStyles ??= new List<Style>();
                    inlineStyles.Add(style);
                }
                else
                {
                    regularStyles ??= new List<Style>();
                    regularStyles.Add(style);
                }
            }
        }

        // Write non-inline styles
        if (regularStyles != null)
        {
            WriteStyles(renderer, context, regularStyles, null);
        }

        // Write inline styles
        if (inlineStyles != null)
        {
            WriteStyles(renderer, context, inlineStyles, "inline");
        }
    }

    private static void WriteStyles(IHtmlRenderer renderer, GlobalContext context, IEnumerable<Style> inline, string? inlineAttr)
    {
        renderer.StartElement("style")
            .Attr("inline", inlineAttr)
            .Attr("type", "text/css");

        foreach (var style in inline)
        {
            style.Renderer(renderer, context);
        }

        renderer.EndElement("style");
    }

    private static void WriteOptionStyles(IHtmlRenderer renderer, GlobalContext context)
    {
        renderer.StartElement("style")
            .Attr("type", "text/css");

        foreach (var style in context.Options.Styles ?? Array.Empty<Style>())
        {
            style.Renderer(renderer, context);
        }

        renderer.EndElement("style");
    }

    private static void WriteMediaQueriesOWA(IHtmlRenderer renderer, GlobalContext context)
    {
        if (!context.Options.ForceOWAQueries)
        {
            return;
        }

        renderer.StartElement("style")
            .Attr("type", "text/css");

        foreach (var mediaQuery in context.GlobalData.Values.OfType<MediaQuery>())
        {
            renderer.Content($"  [owa] {mediaQuery.Rule}");
        }

        renderer.EndElement("style");
    }
}
