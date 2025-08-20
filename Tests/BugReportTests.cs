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

    [Fact]
    public void Should_not_leak_content_between_calls_when_content_has_multiple_roots()
    {
        var renderer = new MjmlRenderer();

        var mjml1 = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-text>Leaked Content</mj-text>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-text>Leaked Content</mj-text>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>";
        _ = renderer.Render(mjml1);

        var mjml2 = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-text>Hello World</mj-text>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>";
        var (html, errors) = renderer.Render(mjml2);

        Assert.Empty(errors);
        Assert.DoesNotContain("Leaked Content", html, StringComparison.Ordinal);
    }
}
