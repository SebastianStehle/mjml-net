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
        var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
        var templateFile = Path.Combine(AppContext.BaseDirectory, "Templates", "include", "about.mjml");

        try
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "npx";
            process.StartInfo.ArgumentList.Add("mjml");
            process.StartInfo.ArgumentList.Add(templateFile);
            process.StartInfo.ArgumentList.Add("-o");
            process.StartInfo.ArgumentList.Add(tempFile);
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            var stderr = process.StandardError.ReadToEnd();
            var stdout = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0 || !File.Exists(tempFile))
            {
                throw new InvalidOperationException(
                    $"npx mjml failed for 'include/about.mjml' (exit code {process.ExitCode}).\nstdout: {stdout}\nstderr: {stderr}");
            }

            return File.ReadAllText(tempFile);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}
