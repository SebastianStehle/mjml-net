using Mjml.Net.Helpers;
using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class FontTests
    {
        [Fact]
        public void Should_render_font()
        {
            var source = @"
<mjml-test body=""false"">
    <mj-head>
        <mj-font name=""Raleway"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
    </mj-head>
    <mj-body>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
        }

        [Fact]
        public void Should_add_font_implicitely()
        {
            var source = @"
<mjml-test body=""false"">
    <mj-body>
        <mj-text font-family=""Ubuntu""></mj-text>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlFileAssert("Components.Outputs.FontUbuntu.html", result);
        }
    }
}
