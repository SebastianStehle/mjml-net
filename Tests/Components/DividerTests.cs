using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class DividerTests
    {
        [Fact]
        public void Should_render_spacer()
        {
            var source = @"<mj-divider />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Divider.html", result);
        }
    }
}
