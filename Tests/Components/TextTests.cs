using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class TextTests
    {
        [Fact]
        public void Should_render_text()
        {
            var source = @"<mj-text>Hello MJML</mj-text>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Text.html", result);
        }

        [Fact]
        public void Should_render_text_with_html()
        {
            var source = @"<mj-text><h1>Hello <span>MJML</span></h1></mj-text>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.TextWithHtml.html", result);
        }

        [Fact]
        public void Should_render_text_with_html2()
        {
            var source = @"<mj-text>Hello <br /><br /> MJML</mj-text>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.TextWithHtml2.html", result);
        }
    }
}
