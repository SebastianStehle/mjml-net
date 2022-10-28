using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class DividerTests
    {
        [Fact]
        public void Should_render_divider()
        {
            var source = @"<mj-divider />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Divider.html", result);
        }

        [Fact]
        public void Should_render_without_width_unit()
        {
            var source = @"<mj-divider width=""500""></mj-divider>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.DividerWithoutWidthUnit.html", result);
        }
    }
}
