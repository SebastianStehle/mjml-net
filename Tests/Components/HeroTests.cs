using Tests.Internal;

namespace Tests.Components;

public class HeroTests
{
    [Fact]
    public void Should_render_hero()
    {
        var source = """
            <mj-hero>
            </mj-hero>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Hero.html", result);
    }

    [Fact]
    public void Should_render_hero_without_width_unit()
    {
        var source = """
            <mj-hero width="500">
            </mj-hero>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Hero.html", result);
    }

    [Fact]
    public void Should_render_hero_with_child()
    {
        var source = """
             <mj-hero>
                <mj-divider />
            </mj-hero>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HeroDivider.html", result);
    }

    [Fact]
    public void Should_render_hero_with_children()
    {
        var source = """
            <mj-hero>
              <mj-divider />
              <mj-divider />
            </mj-hero>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HeroDividers.html", result);
    }

    [Fact]
    public void Should_render_fixed_height_hero_with_height_less_padding()
    {
        var source = """
            <mj-hero mode="fixed-height" height="469px" background-width="600px" background-height="469px"background-color="#2a3448" padding="100px 0px">
                <mj-divider />
                <mj-divider />
            </mj-hero>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HeroFixedHeight.html", result);
    }
}
