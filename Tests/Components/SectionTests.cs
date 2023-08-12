using Tests.Internal;
using Xunit;

namespace Tests.Components;

public class SectionTests
{
    [Fact]
    public void Should_render_section()
    {
        var source = @"<mj-section></mj-section>";

        var result = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Section.html", result);
    }

    [Fact]
    public void Should_render_section_with_background_color()
    {
        var source = @"<mj-section background-color=""red""></mj-section>";

        var result = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.SectionWithBackgroundColor.html", result);
    }

    [Fact]
    public void Should_render_section_with_background_image()
    {
        var source = @"<mj-section background-url=""https://picsum.photos/600/300""></mj-section>";

        var result = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.SectionWithBackgroundImage.html", result);
    }

    [Fact]
    public void Should_render_sections_with_columns()
    {
        var source = @"
<mj-section>
    <mj-column background-color=""#ff0000""></mj-column>
    <mj-column background-color=""#00ff00""></mj-column>
    <mj-column background-color=""#0000ff""></mj-column>
</mj-section>";

        var result = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.SectionWithColumns.html", result);
    }

    [Fact]
    public void Should_render_sections_with_groups()
    {
        var source = @"
<mj-section>
    <mj-group background-color=""#ff0000""></mj-group>
    <mj-group background-color=""#00ff00""></mj-group>
    <mj-group background-color=""#0000ff""></mj-group>
</mj-section>";

        var result = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.SectionWithGroups.html", result);
    }
}
