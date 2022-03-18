using Mjml.Net.Helpers;
using Xunit;

namespace Tests
{
    public class PreviewTests
    {
        [Fact]
        public void Should_render_preview()
        {
            var source = @"
<mjml-test>
    <mj-head>
        <mj-preview>Hello MJML</mj-preview>
    </mj-head>
    <mj-body>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new PreviewHelper());

            AssertHelpers.HtmlAssert(TestHelper.GetContent("Preview.html"), result);
        }
    }
}
