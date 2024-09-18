using Tests.Internal;

namespace Tests.Components;

public class ListTests
{
    [Fact]
    public void Should_render_lists()
    {
        var source = @"
<mj-list>
    <mj-li>List item one.</mj-li>
    <mj-li>List item two.</mj-li>
    <mj-li>List item three.</mj-li>
    <mj-li>List item four.</mj-li>
</mj-list>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.List.html", result);
    }
}
