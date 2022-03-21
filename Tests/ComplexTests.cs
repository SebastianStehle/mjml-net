using System.Diagnostics;
using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class ComplexTests
    {
        private static readonly HashSet<string> Ignore = new HashSet<string>
        {
            "ugg-royale.mjml" // Carousel
        };

        public static IEnumerable<object[]> Templates()
        {
            var files = Directory.GetFiles("Templates", "*.mjml").Select(x => new FileInfo(x));

            return files.Where(x => !Ignore.Contains(x.Name)).Select(x => new[] { x.Name });
        }

        [Theory]
        [MemberData(nameof(Templates))]
        public void Should_render_template(string template)
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

                var expected = File.ReadAllText(tempFile);

                var source = File.ReadAllText($"Templates/{template}");

                var result = new MjmlRenderer().Render(source, new MjmlOptions
                {
                    Beautify = true,
                    // Cleanup XML, because some are broken.
                    Lax = true
                }).Html;

                // Fix whitespaces for easier comparison.
                result = result.Replace("&amp;#160;", " ", StringComparison.Ordinal);

                AssertHelpers.HtmlAssert(template, result, expected);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
