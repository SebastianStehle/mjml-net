using Mjml.Net.Helpers;
using Tests.Properties;
using Xunit;

namespace Tests
{
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
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new StyleHelper());

            AssertHelpers.HtmlAssert(Resources.Style, result);
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
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new StyleHelper());

            AssertHelpers.HtmlAssert(Resources.Style, result);
        }
    }
}
