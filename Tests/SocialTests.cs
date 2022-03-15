using Xunit;

namespace Tests
{
    public class SocialTests
    {
        [Fact]
        public void Should_render_empty_social()
        {
            var source = @"<mj-social />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("SocialEmpty.html"), result);
        }
    }
}
