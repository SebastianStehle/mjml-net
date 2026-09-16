using Mjml.Net;

namespace Tests;

public class RenderToWriterTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_write_same_html_as_string_result(bool beautify)
    {
        var source = File.ReadAllText(Path.Combine("Templates", "amario.mjml"));

        var renderer = new MjmlRenderer();
        var options = new MjmlOptions { Beautify = beautify };

        var expected = renderer.Render(source, options);

        var writer = new StringWriter();
        var errors = renderer.Render(source, writer, options);

        Assert.Equal(expected.Html, writer.ToString());
        Assert.Equal(expected.Errors.Count, errors.Count);
    }
}
