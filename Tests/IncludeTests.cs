using System.Diagnostics;
using Mjml.Net;
using Mjml.Net.Validators;
using Tests.Internal;
using Xunit;

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

        var fileStore = new InMemoryFileLoader();

        foreach (var file in Directory.GetFiles("Templates/include", "*.mjml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)))
        {
            fileStore.Add(file.Name, File.ReadAllText(file.FullName));
        }

        var options = new MjmlOptions
        {
            FileLoader = fileStore,

            // Easier for debugging errors.
            Beautify = true,

            // Use validation, so that we also catch errors here.
            Validator = StrictValidator.Instance
        };

        var (html, errors) = new MjmlRenderer().Render(source, options);

        Assert.Empty(errors.Where(x => x.Type != ValidationErrorType.UnknownAttribute));

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
