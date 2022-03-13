using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class TextTests
    {
        [Fact]
        public void Should_render_text()
        {
            var source = @"
<mjml>
    <mj-body>
        <mj-text>
            <h1>
                Hey Title!
            </h1>
        </mj-text>
    </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.Text, result);
        }
    }
}
