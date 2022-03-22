using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class SocialTests
    {
        [Fact]
        public void Should_render_empty_social()
        {
            var source = @"<mj-social />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.SocialEmpty.html", result);
        }

        [Fact]
        public void Should_render_raw_social()
        {
            var source = @"
<mj-social font-size=""15px"" icon-size=""30px"" mode=""horizontal"">
    <mj-raw>
        Hello MJML
    </mj-raw>
</mj-social>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.SocialRaw.html", result);
        }

        [Fact]
        public void Should_render_social()
        {
            var source = @"
<mj-social font-size=""15px"" icon-size=""30px"" mode=""horizontal"">
    <mj-social-element name=""facebook"" href=""https://mjml.io/"">
        Facebook
    </mj-social-element>
    <mj-social-element name=""google"">
        Google
    </mj-social-element>
    <mj-social-element name=""twitter"" href=""https://mjml.io/"" />
</mj-social>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Social.html", result);
        }
    }
}
