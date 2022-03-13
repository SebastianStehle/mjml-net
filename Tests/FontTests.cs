using Mjml.Net;
using System;
using System.Linq;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class FontTests
    {
        [Fact]
        public void Should_render_font()
        {
            var source = @"
 <mjml>
  <mj-head>
    <mj-font name=""Raleway"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            TestHelpers.TrimmedContains(Resources.Font, result);
        }
    }
}
