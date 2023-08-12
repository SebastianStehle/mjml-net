using System.Diagnostics;

namespace Mjml.Net.Benchmarking;

public static class TestRunner
{
    private static readonly MjmlOptions Options = new MjmlOptions { Beautify = true };

    public static void Run(int numberOfIterations)
    {
        var mjmlRenderer = new MjmlRenderer();
        var mjmlTemplates = Directory.GetFiles("./Templates/", "*.mjml");

        foreach (var mjmlTemplatePath in mjmlTemplates)
        {
            try
            {
                var fileName = Path.GetFileName(mjmlTemplatePath);

                Console.WriteLine($"\n=============================");
                Console.WriteLine($" {fileName}");
                Console.WriteLine($" {mjmlTemplatePath}");
                Console.WriteLine($"=============================");

                var input = File.ReadAllText(mjmlTemplatePath);

                for (var i = 0; i < numberOfIterations; i++)
                {
                    Run(input, mjmlRenderer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private static void Run( string input, MjmlRenderer mjmlRenderer)
    {
        var watch = Stopwatch.StartNew();

        var html = mjmlRenderer.Render(input, Options).Html;

        watch.Stop();

        Console.WriteLine("* Elapsed after {0}ms. Length {1}", watch.Elapsed.TotalMilliseconds, html.Length);
    }
}
