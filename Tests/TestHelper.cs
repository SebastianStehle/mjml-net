using Mjml.Net;

namespace Tests
{
    public static class TestHelper
    {
        public static string Render(string source)
        {
            var renderer = new MjmlRenderer().Add(new TestComponent());

            return renderer.Render(source, new MjmlOptions
            {
                Beautify = true
            });
        }

        public static string Render(string source, params IHelper[] helpers)
        {
            var renderer = new MjmlRenderer().Add(new TestComponent()).ClearHelpers();

            foreach (var helper in helpers)
            {
                renderer.Add(helper);
            }

            return renderer.Render(source, new MjmlOptions
            {
                Beautify = true
            });
        }
    }
}
