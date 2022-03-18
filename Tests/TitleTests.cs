using Mjml.Net.Helpers;
using Xunit;

namespace Tests
{
    public class TitleTests
    {
        [Fact]
        public void Should_render_title()
        {
            var source = @"
<mjml-test body=""false"">
    <mj-head>
        <mj-title>Hello MJML</mj-title>
    </mj-head>
    <mj-body>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new TitleHelper());

            AssertHelpers.HtmlAssert(TestHelper.GetContent("Title.html"), result);
        }
    }
}
