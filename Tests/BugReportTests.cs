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
    public void Render_should_not_leak_content_between_calls()
    {
        var renderer = new MjmlRenderer();

        // ok
        var one = "<mjml>\n<mj-head>\n<mj-title>Hello World Example</mj-title>\n</mj-head>\n<mj-body>\n<mj-section>\n<mj-column>\n<mj-text>\nHello World! <span data-content-type=\"textunit\" data-content-key=\"test_key\" data-content-channelid=\"chanl_00000000-0000-0000-0000-000000000002\" data-content-language=\"en-GB\" data-content-is-draft=\"false\" data-content-limit=\"0\" data-content-is-inline=\"true\"></span> [[TRANSLATED!: 'test_key' in 'en-GB']]\n</mj-text>\n</mj-column>\n</mj-section>\n</mj-body>\n</mjml>";
        _ = renderer.Render(one);

        // ok
        var two = "<mjml>\n<mj-head>\n<mj-title>Hello World Example</mj-title>\n</mj-head>\n<mj-body>\n<mj-section>\n<mj-column>\n<mj-text>\nHello World! <span data-content-type=\"textunit\" data-content-key=\"test_key\" data-content-channelid=\"chanl_00000000-0000-0000-0000-000000000002\" data-content-language=\"en-GB\" data-content-is-draft=\"false\" data-content-limit=\"0\" data-content-is-inline=\"true\"></span> [[TRANSLATED!: 'test_key' in 'en-GB']]\n</mj-text>\n</mj-column>\n</mj-section>\n</mj-body>\n</mjml><mjml>\n<mj-head>\n<mj-title>Hello World Example</mj-title>\n</mj-head>\n<mj-body>\n<mj-section>\n<mj-column>\n<mj-text>\nHello World!  [[TRANSLATED!: 'test_key' in 'en-GB']]\n</mj-text>\n</mj-column>\n</mj-section>\n</mj-body>\n</mjml>";
        _ = renderer.Render(two);

        // fails to compile (as expected)
        var three = "<mjml>\n<mj-head>\n<mj-title>Hello World Example</mj-title>\n</mj-head>\n<mj-body>\n<mj-section>\n<mj-column>\n<mj-text>\nHello World! <span data-content-type=\"textunit\" data-content-key=\"test_key\" data-content-channelid=\"chanl_00000000-0000-0000-0000-000000000002\" data-content-language=\"en-GB\" data-content-is-draft=\"false\" data-content-limit=\"0\" data-content-is-inline=\"true\"></span> [[TRANSLATED!: 'test_key' in 'en-GB']]\n</mj-text>\n</mj-column>\n</mj-section>\n</mj-body>\n</mjml>\nthis is some new content";
        _ = renderer.Render(three);

        // BAD - this one shouldn't include "data-content-key"
        var four = "<mjml>\n<mj-head>\n<mj-title>Hello World Example</mj-title>\n</mj-head>\n<mj-body>\n<mj-section>\n<mj-column>\n<mj-text>\nHello World!  [[TRANSLATED!: 'test_key' in 'en-GB']]\n</mj-text>\n</mj-column>\n</mj-section>\n</mj-body>\n</mjml>\n<mj-text font-family=\"Helvetica\" color=\"#F45E43\"><p>this is some new content</p></mj-text>";
        var (html, errors) = renderer.Render(four);

        Assert.Empty(errors);
        Assert.DoesNotContain("data-content-key", html, StringComparison.Ordinal);
    }
}
