using Mjml.Net;

namespace Tests.Components;

public class BreakpointTests
{
    private const string Body = "<mj-body><mj-section><mj-column><mj-text>Text</mj-text></mj-column></mj-section></mj-body>";

    [Fact]
    public void Should_use_breakpoint_from_head()
    {
        var result = new MjmlRenderer().Render($"<mjml><mj-head><mj-breakpoint width=\"320px\" /></mj-head>{Body}</mjml>").Html;

        Assert.Contains("min-width:320px", result, StringComparison.Ordinal);
    }

    [Fact]
    public void Should_not_modify_options_with_breakpoint()
    {
        var renderer = new MjmlRenderer();
        var options = new MjmlOptions();

        renderer.Render($"<mjml><mj-head><mj-breakpoint width=\"320px\" /></mj-head>{Body}</mjml>", options);

        var result = renderer.Render($"<mjml>{Body}</mjml>", options).Html;

        Assert.Equal("480px", options.Breakpoint);
        Assert.Contains("min-width:480px", result, StringComparison.Ordinal);
    }
}
