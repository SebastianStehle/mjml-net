using Mjml.Net.Helpers;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class BreakpointTests
    {
        [Fact]
        public void Should_render_breakpoint()
        {
            var source = @"
<mjml-test>
  <mj-head>
    <mj-breakpoint width=""300px"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new BreakpointHelper());

            AssertHelpers.HtmlAssert(Resources.Breakpoint, result);
        }
    }
}
