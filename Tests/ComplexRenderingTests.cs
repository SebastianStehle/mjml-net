using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class ComplexRenderingTests
    {
        [Fact]
        public void Should_render_body_only()
        {
            var source = @"
<mjml>
    <mj-body>
    </mj-body>
 </mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            }).Html;

            AssertHelpers.TrimmedContains("</body>", result);
        }

        [Fact]
        public void Should_render_amario()
        {
            var source = TestHelper.GetContent("Amario.mjml");

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            }).Html;

            AssertHelpers.HtmlFileAsset("Amario.html", result, true);
        }
    }
}
