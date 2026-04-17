using System.Collections.Concurrent;
using System.Diagnostics;
using Mjml.Net;
using Mjml.Net.Validators;
using Tests.Internal;

namespace Tests;

public class ComplexTests
{
    private static readonly ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

    private static readonly MjmlOptions Options = new MjmlOptions
    {
        // Easier for debugging errors.
        Beautify = true,

        // Use validation, so that we also catch errors here.
        Validator = StrictValidator.Instance
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
        foreach (var file in Directory.GetFiles("Templates", "*.mjml", SearchOption.TopDirectoryOnly).Select(x => new FileInfo(x)))
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
            var renderedNode = CompileWithNode(template);
            var renderedNet = CompileWithNet(template);

            AssertHelpers.HtmlAssert(template, renderedNet, renderedNode, true);
        });
    }

    private static string CompileWithNet(string template)
    {
        var source = File.ReadAllText($"Templates/{template}");

        var (html, errors) = new MjmlRenderer().Render(source, Options);

        Assert.DoesNotContain(errors, x => x.Type != ValidationErrorType.UnknownAttribute);
        return html;
    }

    private static string CompileWithNode(string template)
    {
        return Cache.GetOrAdd(template, fileName =>
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.html");
            var templateFile = Path.Combine(AppContext.BaseDirectory, "Templates", fileName);

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
                        $"npx mjml failed for '{fileName}' (exit code {process.ExitCode}).\nstdout: {stdout}\nstderr: {stderr}");
                }

                return File.ReadAllText(tempFile);
            }
            finally
            {
                File.Delete(tempFile);
            }
        });
    }
}
