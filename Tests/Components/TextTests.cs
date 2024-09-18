using Mjml.Net;
using Tests.Internal;

namespace Tests.Components;

public class TextTests
{
    [Fact]
    public void Should_render_text()
    {
        var source = @"<mj-text>Hello MJML</mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Text.html", result);
    }

    [Fact]
    public void Should_render_text_with_whitespace()
    {
        var source = @"<mj-text>Hello&nbsp;MJML</mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWhitespace.html", result);
    }

    [Fact]
    public void Should_render_text_with_html()
    {
        var source = @"<mj-text><h1>Hello <span>MJML</span></h1></mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWithHtml.html", result);
    }

    [Fact]
    public void Should_render_text_with_html2()
    {
        var source = @"<mj-text>Hello <br /><br /> MJML</mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWithHtml2.html", result);
    }

    [Fact]
    public void Should_render_text_with_entity()
    {
        var source = @"<mj-text>Hello ’MJML’</mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWithEntity.html", result);
    }

    [Fact]
    public void Should_render_raw_text_with_whitespace()
    {
        var source = @"
<mj-text>
    <p>Hello&nbsp;MJML</p>
</mj-text>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextRawWhitespace.html", result);
    }

    [Fact]
    public void Should_render_text_with_html_and_whitespace()
    {
        var source = @"<mj-text>This <strong>should</strong> respect <strong>whitespaces.</strong> after the <strong>HTML Tags</strong></mj-text>";

        var renderer = new MjmlRenderer().Add<TestComponent>();

        var result = renderer.Render(source, new MjmlOptions()
        {
            Beautify = false,
        }).Html;

        var expected = TestHelper.GetContent("Components.Outputs.TextWithHtmlAndWhitespace.html");

        Assert.Equal(expected.Trim(), result.Trim());
    }
}
