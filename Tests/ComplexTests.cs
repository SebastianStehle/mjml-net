using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class ComplexTests
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
            var source = TestHelper.GetContent("Tests.Amario.mjml");

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            }).Html;

            AssertHelpers.HtmlFileAsset("Tests.Amario.html", result);
        }
    }
}
