using Tests.Internal;
using Xunit;

namespace Tests
{
    public class SpacerTests
    {
        [Fact]
        public void Should_render_spacer()
        {
            var source = @"<mj-spacer />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Spacer.html", result);
        }

        [Fact]
        public void Should_render_inline_just_normal_as_fallback()
        {
            var source = @"<mj-spacer height=""100px"" />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("SpacerWithHeight.html", result);
        }
    }
}
