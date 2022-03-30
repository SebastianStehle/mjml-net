using System.Collections.Concurrent;
using System.Diagnostics;
using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class ComplexTests
    {
        private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        private static readonly MjmlOptions Options = new MjmlOptions
        {
            // Easier for debugging errors.
            Beautify = true,

            // Cleanup XML, because some are broken.
            Lax = true
        };

        public static IEnumerable<string> Cultures()
        {
            yield return "en-US";
            yield return "de-DE";
            yield return "es-ES";
            yield return string.Empty;
        }

        public static IEnumerable<string> Templates()
        {
            foreach (var file in Directory.GetFiles("Templates", "*.mjml").Select(x => new FileInfo(x)))
            {
                yield return file.Name;
            }
        }

        public static IEnumerable<object[]> TestCases()
        {
            foreach (var file in Templates())
            {
                foreach (var culture in Cultures())
                {
                    yield return new object[] { file, culture };
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public void Should_render_template(string template, string culture)
        {
            TestHelper.TestWithCulture(culture, () =>
            {
                var expected = CompileWithNode(template);

                var result = CompileWithNet(template);

                AssertHelpers.HtmlAssert(template, result, expected, true);
            });
        }

        private static string CompileWithNet(string template)
        {
            var source = File.ReadAllText($"Templates/{template}");

            return new MjmlRenderer().Render(source, Options).Html;
        }

        private static string CompileWithNode(string template)
        {
            return Cache.GetOrAdd(template, _ =>
            {
                var tempFile = Guid.NewGuid().ToString();

                try
                {
                    var process = new Process();
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.FileName = "npx";
                    process.StartInfo.Arguments = $"mjml Templates/{template} -o {tempFile}";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                    process.WaitForExit();

                    return File.ReadAllText(tempFile);
                }
                finally
                {

                    File.Delete(tempFile);
                }
            });
        }
    }
}
