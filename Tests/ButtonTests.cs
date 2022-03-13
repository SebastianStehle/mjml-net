using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class ButtonTests
    {
        [Fact]
        public void Should_render_button()
        {
            var source = @"
<mjml>
    <mj-body>
        <mj-button font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
            Button
        </mj-button>
    </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            TestHelpers.TrimmedContains(Resources.Button, result);
        }

        [Fact]
        public void Should_render_button_link()
        {
            var source = @"
<mjml>
    <mj-body>
        <mj-button href=""https://mjml.io/"" font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
            Button Link
        </mj-button>
    </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            TestHelpers.TrimmedContains(Resources.ButtonLink, result);
        }
    }
}
