using Mjml.Net;
using Mjml.Net.Helpers;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class TitleTests
    {
        [Fact]
        public void Should_render_title()
        {
            var source = @"
 <mjml plain=""plain"">
  <mj-head>
    <mj-title>Hello MJML</mj-title>
  </mj-head>
  <mj-body>
  </mj-body>
</mjml>
";

            var result = TestHelper.Render(source, new TitleHelper());

            AssertHelpers.HtmlAssert(Resources.Title, result);
        }
    }
}
