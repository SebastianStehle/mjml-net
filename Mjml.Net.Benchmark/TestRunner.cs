using System;
using System.Diagnostics;

namespace Mjml.Net.Benchmarking
{
    public static class TestRunner
    {
        public static void Run()
        {
            var mjmlRenderer = new MjmlRenderer();

            Console.WriteLine("Many Heroes");

            for (var i = 0; i < 20; i++)
            {
                Run("ManyHeroes.mjml", mjmlRenderer);
            }

            Console.WriteLine("---");
            Console.WriteLine("Amario");

            for (var i = 0; i < 20; i++)
            {
                Run("Amario.mjml", mjmlRenderer);
            }

            return;
            Console.WriteLine("---");
            Console.WriteLine("Austin");

            for (var i = 0; i < 20; i++)
            {
                Run("Austin.mjml", mjmlRenderer);
            }

            Console.WriteLine("---");
            Console.WriteLine("Sphero");

            for (var i = 0; i < 20; i++)
            {
                Run("Sphero.mjml", mjmlRenderer);
            }
        }

        private static void Run(string file, MjmlRenderer mjmlRenderer)
        {
            var text = File.ReadAllText($"./Templates/{file}");

            var watch = Stopwatch.StartNew();

            var html = mjmlRenderer.Render(text, new MjmlOptions
            {
                Beautify = false
            }).Html;

            watch.Stop();

            File.WriteAllText($"{file}.html", html);

            Console.WriteLine("* Elapsed after {0}ms. Length {1}", watch.Elapsed.TotalMilliseconds, html.Length);
        }
    }
}
