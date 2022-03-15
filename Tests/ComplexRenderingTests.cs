using Mjml.Net;
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
            });

            AssertHelpers.TrimmedContains("</body>", result);
        }
    }
}
