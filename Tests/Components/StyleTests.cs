using Mjml.Net;
using Mjml.Net.Helpers;
using Tests.Internal;

namespace Tests.Components;

public class StyleTests
{
    [Fact]
    public void Should_render_style()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style>
                  .red-text div {
                    color: red !important;
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="red-text">
                    <div style="font-weight: bold"></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Style.html", result);
    }

    [Fact]
    public async Task Should_render_inline()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style inline="inline">
                  .red-text div {
                    color: red !important;
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="red-text">
                    <div style="font-weight: bold; font-size: 1.5rem;"></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors =
            [
                AngleSharpPostProcessor.Default
            ],
            Beautify = true
        }, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInline.html", result);
    }

    [Fact]
    public async Task Should_render_inline2()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style inline="inline">
                  .red-text div {
                    mso-hide: true
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="red-text">
                    <div></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors =
            [
                AngleSharpPostProcessor.Default
            ],
            Beautify = true
        }, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInline2.html", result);
    }

    [Fact]
    public async Task Should_render_inline3()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style inline="inline">
                  .break-text div {
                    word-break: break-word
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="break-text">
                    <div></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors =
            [
                AngleSharpPostProcessor.Default
            ],
            Beautify = true
        }, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInline3.html", result);
    }

    [Fact]
    public async Task Should_render_inline4_with_shorthand()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-style inline="inline">
                  a { letter-spacing: 1px; }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-button href="https://mjml.io">MJML</mj-button>
              </mj-body>
            </mjml>
            """;

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors =
            [
                AngleSharpPostProcessor.Default
            ],
            Beautify = true
        }, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInline4.html", result);
    }

    [Fact]
    public void Should_render_inline_fallback()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style inline="inline">
                  .red-text div {
                    color: red !important;
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="red-text">
                    <div style="font-weight: bold"></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInlineFallback.html", result);
    }

    [Fact]
    public async Task Should_keep_non_inline_styles_when_inlining()
    {
        var source = """
            <mjml-test>
              <mj-head>
                <mj-style>
                  .red-text > div {
                    color: blue;
                  }
                </mj-style>
                <mj-style inline="inline">
                  .red-text div {
                    color: red;
                  }
                </mj-style>
              </mj-head>
              <mj-body>
                <mj-raw>
                  <div class="red-text">
                    <div></div>
                  </div>
                </mj-raw>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors =
            [
                AngleSharpPostProcessor.Default
            ],
            Beautify = true
        }, helpers: [new StyleHelper()]);

        // The non-inline style must stay unchanged and must not be applied to the elements.
        Assert.Contains(".red-text > div", result, StringComparison.Ordinal);
        Assert.DoesNotContain("&gt;", result, StringComparison.Ordinal);
        Assert.Contains("<div style=\"color: rgba(255, 0, 0, 1)\"></div>", result, StringComparison.Ordinal);
        Assert.DoesNotContain("color: blue\"", result, StringComparison.Ordinal);
    }
}
