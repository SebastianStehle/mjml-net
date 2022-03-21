using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class TextTests
    {
        [Fact]
        public void Should_render_text()
        {
            var source = @"<mj-text>Hey Title!</mj-text>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.Text.html", result);
        }

        [Fact]
        public void Should_render_text_with_html()
        {
            var source = @"<mj-text><h1>Hey <span>Title!</span></h1></mj-text>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.TextWithHtml.html", result);
        }
    }
}
