using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class SpacerTests
    {
        [Fact]
        public void Should_render_spacer()
        {
            var source = @"
<mjml>
  <mj-body>
    <mj-spacer />
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.Spacer, result);
        }

        [Fact]
        public void Should_render_inline_just_normal_as_fallback()
        {
            var source = @"
<mjml>
  <mj-body>
    <mj-spacer height=""100px"" />
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.SpacerWithHeight, result);
        }
    }
}
