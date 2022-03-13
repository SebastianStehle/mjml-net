﻿using Mjml.Net.Helpers;
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
<mjml-test>
  <mj-head>
    <mj-font name=""Raleway"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlAssert(Resources.Font, result);
        }
    }
}
