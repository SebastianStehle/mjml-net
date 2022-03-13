using Mjml.Net;
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
 <mjml>
  <mj-head>
    <mj-breakpoint width=""300px"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            TestHelpers.TrimmedContains(Resources.Breakpoint, result);
        }
    }
}
