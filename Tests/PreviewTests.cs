using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class PreviewTests
    {
        [Fact]
        public void Should_render_preview()
        {
            var source = @"
 <mjml>
  <mj-head>
    <mj-preview>Hello MJML</mj-preview>
  </mj-head>
  <mj-body>
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.Preview, result);
        }
    }
}
