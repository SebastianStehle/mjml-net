using Html.Net;
using Mjml.Net;
using Mjml.Net.Helpers;
using Tests.Internal;
using Xunit;

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

        var result = TestHelper.Render(source, new StyleHelper());

        AssertHelpers.HtmlFileAssert("Components.Outputs.Style.html", result);
    }

    [Fact]
    public void Should_render_inline_just_normal_as_fallback()
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

        var result = TestHelper.Render(source, new StyleHelper());

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInline.html", result);
    }

    [Fact]
    public void Should_render_inline()
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

        var options = new MjmlOptions
        {
            PostProcessors =
            [
                InlineProcessor.Instance
            ],
            Beautify = true
        };

        var result = TestHelper.Render(source, options, new StyleHelper());

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInlined.html", result);
    }
}
