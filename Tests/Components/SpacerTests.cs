using Tests.Internal;

namespace Tests.Components;

public class SpacerTests
{
    [Fact]
    public void Should_render_spacer()
    {
        var source = @"<mj-spacer />";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Spacer.html", result);
    }

    [Fact]
    public void Should_render_inline_just_normal_as_fallback()
    {
        var source = @"<mj-spacer height=""100px"" />";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.SpacerWithHeight.html", result);
    }
}
