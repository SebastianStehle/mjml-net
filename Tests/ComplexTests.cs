using System.Diagnostics;
using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class ComplexTests
    {
        public static IEnumerable<object[]> Templates()
        {
            var files = Directory.GetFiles("Templates", "*.mjml");

            return files.Select(x => new[] { new FileInfo(x).Name });
        }

        [Theory(Skip = "Too expensive")]
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
                    Beautify = true
                }).Html;

                AssertHelpers.HtmlAssert(template, result, expected);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
