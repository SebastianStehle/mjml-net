using System.IO;
using Mjml.Net;

namespace Tests
{
    public static class TestHelper
    {
        public static string Render(string source)
        {
            var renderer = new MjmlRenderer().Add<TestComponent>();

            return renderer.Render(source, new MjmlOptions
            {
                Beautify = true
            }).Html;
        }

        public static string Render(string source, params IHelper[] helpers)
        {
            var renderer = new MjmlRenderer().Add<TestComponent>().ClearHelpers();

            foreach (var helper in helpers)
            {
                renderer.Add(helper);
            }

            return renderer.Render(source, new MjmlOptions
            {
                Beautify = true
            }).Html;
        }

        public static string GetContent(string content)
        {
            var stream = typeof(TestHelper).Assembly.GetManifestResourceStream($"Tests.Tests.{content}")!;

            return new StreamReader(stream).ReadToEnd();
        }
    }
}
