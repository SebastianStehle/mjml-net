using System.Runtime.CompilerServices;
using System.Text;
using Mjml.Net.Internal;

namespace Mjml.Net;

public sealed partial class MjmlRenderContext : IHtmlRenderer, IHtmlAttrRenderer
{
    private readonly RenderStack<RenderBuffer> buffers = new RenderStack<RenderBuffer>();
    private readonly HashSet<string> analyzedFonts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    internal RenderBuffer Buffer
    {
        get => buffers.Current!;
    }

    public StringBuilder StringBuilder
    {
        get => Buffer.StringBuilder!;
    }

    private void ClearRenderData()
    {
        analyzedFonts.Clear();

        buffers.Clear();
    }

    public void ReturnStringBuilder(StringBuilder stringBuilder)
    {
        DefaultPools.StringBuilders.Return(stringBuilder);
    }

    public void StartBuffer()
    {
        buffers.Push(new RenderBuffer(DefaultPools.StringBuilders.Get(), mjmlOptions.Beautify));
    }

    public StringBuilder? EndBuffer()
    {
        Buffer.FlushAll();

        return buffers.Pop()?.StringBuilder;
    }

    public void RenderHelpers(HelperTarget target)
    {
        foreach (var helper in mjmlRenderer.Helpers)
        {
            helper.Render(this, target, context);
        }
    }

    public IHtmlAttrRenderer StartElement(string elementName, bool close = false)
    {
        Buffer.StartElement(elementName, close);
        return this;
    }

    public IHtmlAttrRenderer Attr(string name, string? value)
    {
        DetectFontFamily(name, value);

        Buffer.Attr(name, value);
        return this;
    }

    public IHtmlAttrRenderer Attr(string name, [InterpolatedStringHandlerArgument("", "name")] ref AttrInterpolatedStringHandler value)
    {
        Buffer.EndAttr();
        return this;
    }

    public IHtmlAttrRenderer StartAttr(string name)
    {
        Buffer.StartAttr(name);
        return this;
    }

    public IHtmlClassRenderer Class(string? value)
    {
        Buffer.Class(value);
        return this;
    }

    public IHtmlClassRenderer Class([InterpolatedStringHandlerArgument("")] ref ClassNameInterpolatedStringHandler value)
    {
        // Starting the class is done by the interpolation handler.
        Buffer.EndClass();
        return this;
    }

    public IHtmlClassRenderer StartClass()
    {
        Buffer.StartClass();
        return this;
    }

    public IHtmlStyleRenderer Style(string name, string? value)
    {
        DetectFontFamily(name, value);

        Buffer.Style(name, value);
        return this;
    }

    public IHtmlStyleRenderer Style(string name, [InterpolatedStringHandlerArgument("", "name")] ref StyleInterpolatedStringHandler value)
    {
        // Ending the style is done by the interpolation handler.
        Buffer.EndStyle();
        return this;
    }

    public IHtmlStyleRenderer StartStyle(string name)
    {
        Buffer.StartStyle(name);
        return this;
    }

    public void EndElement(string elementName)
    {
        Buffer.EndElement(elementName);
    }

    public void Content([InterpolatedStringHandlerArgument("")] ref TextInterpolatedStringHandler value)
    {
        Buffer.WriteLineEnd();
    }

    public void Content(string? value)
    {
        Buffer.Content(value);
    }

    public void Content(InnerTextOrHtml? value)
    {
        Buffer.Content(value);
    }

    public void StartText()
    {
        Buffer.StartText();
    }

    public void Plain(StringBuilder? value)
    {
        Buffer.Plain(value);
    }

    public void Plain(InnerTextOrHtml value)
    {
        Buffer.Plain(value);
    }

    public void Plain(ReadOnlySpan<char> value)
    {
        Buffer.Plain(value);
    }

    public void StartConditional(string content)
    {
        Buffer.StartConditional(content);
    }

    public void EndConditional(string content)
    {
        Buffer.EndConditional(content);
    }

    private void DetectFontFamily(string name, string? value)
    {
        if (name != "font-family" || string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!analyzedFonts.Add(value))
        {
            return;
        }

        var hasMultipleFonts = value.Contains(',', StringComparison.OrdinalIgnoreCase);

        if (hasMultipleFonts)
        {
            // If we have multiple fonts it is faster than a string.Split, because we can avoid allocations.
            foreach (var (key, font) in mjmlOptions.Fonts)
            {
                if (value.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    context.SetGlobalData(key, font, true);
                }
            }
        }
        else
        {
            // Fast track for a single font.
            if (mjmlOptions.Fonts.TryGetValue(value, out var font))
            {
                context.SetGlobalData(value, font, true);
            }
        }
    }
}
