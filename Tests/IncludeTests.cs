using System.Diagnostics;
using Mjml.Net;
using Mjml.Net.Validators;
using Tests.Internal;

namespace Tests;

public class IncludeTests
{
    [Fact]
    public void Should_render()
    {
        var renderedNode = CompileWithNode();
        var renderedNet = CompileWithNet();

        AssertHelpers.HtmlAssert("include", renderedNet, renderedNode, true);
    }

    private static string CompileWithNet()
    {
        var source = File.ReadAllText($"Templates/include/about.mjml");

        var files = new Dictionary<string, string>();

        foreach (var file in Directory.GetFiles("Templates/include", "*.mjml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)))
        {
            files.Add(file.Name, File.ReadAllText(file.FullName));
        }

        var options = new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files),

            // Easier for debugging errors.
            Beautify = true,

            // Use validation, so that we also catch errors here.
            Validator = StrictValidator.Instance
        };

        var (html, errors) = new MjmlRenderer().Render(source, options);

        Assert.DoesNotContain(errors, x => x.Type != ValidationErrorType.UnknownAttribute);
        return html;
    }

    private static string CompileWithNode()
    {
        var tempFile = Guid.NewGuid().ToString();

        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "npx";
            process.StartInfo.Arguments = $"mjml Templates/include/about.mjml -o {tempFile}";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
            process.WaitForExit();

            return File.ReadAllText(tempFile);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
