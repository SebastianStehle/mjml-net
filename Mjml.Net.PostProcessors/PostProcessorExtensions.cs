using Mjml.Net.Components;

namespace Mjml.Net;

public static class PostProcessorExtensions
{
    public static MjmlOptions WithPostProcessors(this MjmlOptions options)
    {
        options.PostProcessors = [AngleSharpPostProcessor.Default];
        return options;
    }

    public static IMjmlRenderer AddHtmlAttributes(this IMjmlRenderer renderer)
    {
        renderer.Add<HtmlAttributeComponent>();
        renderer.Add<HtmlAttributesComponent>();
        renderer.Add<SelectorComponent>();
        return renderer;
    }

    public static bool HasProcessor<T>(this MjmlOptions options)
    {
        if (options.PostProcessors == null || options.PostProcessors.Length == 0)
        {
            return false;
        }

        foreach (var processor in options.PostProcessors)
        {
            if (processor is T)
            {
                return true;
            }

            if (processor is INestingPostProcessor nesting && nesting.Has<T>())
            {
                return true;
            }
        }

        return false;
    }
}
