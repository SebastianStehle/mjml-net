using Mjml.Net;
using Mjml.Net.Helpers;
using Tests.Internal;

namespace Tests.Components;

public class StyleTests
{
    [Fact]
    public void Should_render_style()
    {
        var source = @"
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
      <div class=""red-text"">
        <div style=""font-weight: bold""></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Style.html", result);
    }

    [Fact]
    public async Task Should_render_inline()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-style inline=""inline"">
      .red-text div {
        color: red !important;
      }
    </mj-style>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""red-text"">
        <div style=""font-weight: bold; font-size: 1.5rem;""></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

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
    public void Should_render_inline_fallback()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-style inline=""inline"">
      .red-text div {
        color: red !important;
      }
    </mj-style>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""red-text"">
        <div style=""font-weight: bold""></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, helpers: [new StyleHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInlineFallback.html", result);
    }
}
