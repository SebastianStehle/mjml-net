using Mjml.Net.Helpers;
using Tests.Internal;
using Xunit;

namespace Tests.Components
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

            AssertHelpers.HtmlFileAssert("Components.Outputs.Preview.html", result);
        }
    }
}
