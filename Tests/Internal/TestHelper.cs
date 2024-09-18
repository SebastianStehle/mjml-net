using System.Globalization;
using Mjml.Net;

namespace Tests.Internal;

public static class TestHelper
{
    public static RenderResult Render(string source, MjmlOptions? options = null, IHelper[]? helpers = null)
    {
        var renderer = CreateRenderer(helpers);

        return renderer.Render(source, BuildOptions(options));
    }

    public static async Task<RenderResult> RenderAsync(string source, MjmlOptions? options = null, IHelper[]? helpers = null)
    {
        var renderer = CreateRenderer(helpers);

        return await renderer.RenderAsync(source, BuildOptions(options));
    }

    private static MjmlOptions BuildOptions(MjmlOptions? options)
    {
        options ??= new MjmlOptions();

        return options with
        {
            Beautify = true
        };
    }

    private static IMjmlRenderer CreateRenderer(IHelper[]? helpers)
    {
        var renderer =
            new MjmlRenderer()
                .AddList()
                .AddHtmlAttributes()
                .Add<TestComponent>();

        if (helpers != null)
        {
            renderer.ClearHelpers();

            foreach (var helper in helpers)
            {
                renderer.Add(helper);
            }
        }

        return renderer;
    }

    public static string GetContent(string content)
    {
        var stream = typeof(TestHelper).Assembly.GetManifestResourceStream($"Tests.{content}")!;

        return new StreamReader(stream).ReadToEnd();
    }

    public static void TestWithCulture(string cultureCode, Action action)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }
}
