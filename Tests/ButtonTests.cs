using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class ButtonTests
    {
        [Fact]
        public void Should_render_button()
        {
            var source = @"
<mj-button font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
    Button
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(Resources.Button, result);
        }

        [Fact]
        public void Should_render_button_link()
        {
            var source = @"
<mj-button href=""https://mjml.io/"" font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
    Button Link
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(Resources.ButtonLink, result);
        }

        [Fact]
        public void Should_render_button_link_with_rel()
        {
            var source = @"
<mj-button href=""https://mjml.io/"" font-family=""Helvetica"" background-color=""#f45e43"" color=""white"" rel=""relly good"">
    Button Link
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(Resources.ButtonLinkWithRel, result);
        }
    }
}
