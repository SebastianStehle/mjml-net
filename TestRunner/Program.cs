using System.Diagnostics;
using Mjml.Net;

namespace TestRunner
{
    public static class Program
    {
        public static void Main(string[] args)
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
            var text = File.ReadAllText(file);

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
