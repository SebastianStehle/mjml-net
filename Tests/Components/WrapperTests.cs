using Tests.Internal;

namespace Tests.Components;

public class WrapperTests
{
    [Fact]
    public void Should_render_wrapper()
    {
        var source = """
            <mj-wrapper>
                <mj-spacer css-class="class1" />
                <mj-spacer css-class="class2" />
            </mj-wrapper>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Wrapper.html", result);
    }
}
