using Mjml.Net;

namespace Tests;

public class BugReportTests
{
    [Fact]
    public void Should_produce_deterministic_results()
    {
        var expected = RenderSample();

        for (var i = 0; i < 10; i++)
        {
            var actual = RenderSample();

            Assert.Equal(expected, actual);
        }

        static string RenderSample()
        {
            var source = @"
<mj-raw>
            <!-- MJML-COMPONENT-START -->
</mj-raw>
<mj-section>
    <mj-column>
        <mj-button font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
            Don't click me!
        </mj-button>
    </mj-column>
</mj-section>
<mj-raw>
    <!-- MJML-COMPONENT-END -->
</mj-raw>
";
            var (html, _) = new MjmlRenderer().Render(source);
            return html;
        }
    }
}
