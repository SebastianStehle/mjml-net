using Mjml.Net;
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
 <mjml>
  <mj-head>
    <mj-title>Hello MJML<mj-title>
  </mj-head>
  <mj-body>
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            TestHelpers.TrimmedContains(Resources.Title, result);
        }
    }
}
