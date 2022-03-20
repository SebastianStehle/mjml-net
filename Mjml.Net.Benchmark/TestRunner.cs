using System;
using System.Diagnostics;

namespace Mjml.Net.Benchmarking
{
    public static class TestRunner
    {
        public static void Run(int numberOfIterations)
        {
            var mjmlRenderer = new MjmlRenderer();
            var mjmlTemplatePaths = Directory.GetFiles("./Templates/", "*.mjml");

            foreach (var mjmlTemplatePath in mjmlTemplatePaths)
            {
                try
                {
                    if (string.IsNullOrEmpty(mjmlTemplatePath))
                    {
                        continue;
                    }    

                    var fileName = Path.GetFileName(mjmlTemplatePath);

                    Console.WriteLine($"\n=============================");
                    Console.WriteLine($" {fileName}");
                    Console.WriteLine($" {mjmlTemplatePath}");
                    Console.WriteLine($"=============================");

                    for (var i = 0; i < numberOfIterations; i++)
                    {
                        Run(fileName, mjmlTemplatePath, mjmlRenderer);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private static void Run(string fileName, string filePath, MjmlRenderer mjmlRenderer)
        {
            var text = File.ReadAllText(filePath);

            var watch = Stopwatch.StartNew();

            var html = mjmlRenderer.Render(text, new MjmlOptions
            {
                Beautify = false
            }).Html;

            watch.Stop();

            File.WriteAllText(fileName.Replace(".mjml", ".html"), html);

            Console.WriteLine("* Elapsed after {0}ms. Length {1}", watch.Elapsed.TotalMilliseconds, html.Length);
        }
    }
}
