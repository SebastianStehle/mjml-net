using Mjml.Net.Helpers;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class AttributesTests
    {
        [Fact]
        public void Should_render_font_with_attributes()
        {
            var source = @"
<mjml-test body=""false"">
    <mj-head>
        <mj-attributes>
            <mj-font name=""Raleway"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
        </mj-attributes>
        <mj-font />
    </mj-head>
    <mj-body>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlFileAsset("Font.html", result);
        }

        [Fact]
        public void Should_render_font_with_class_attribute()
        {
            var source = @"
<mjml-test body=""false"">
  <mj-head>
    <mj-attributes>
      <mj-class name=""blue"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
    </mj-attributes>
    <mj-font mj-class=""red blue"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlFileAsset("Font.html", result);
        }

        [Fact]
        public void Should_render_font_with_multiple_attributes()
        {
            var source = @"
<mjml-test body=""false"">
  <mj-head>
    <mj-attributes>
      <mj-other />
    </mj-attributes>
    <mj-attributes>
      <mj-font name=""Mono"" href=""https://fonts.googleapis.com/css?family=Mono"" />
    </mj-attributes>
    <mj-attributes>
      <mj-font name=""Raleway"" href=""https://fonts.googleapis.com/css?family=Raleway"" />
    </mj-attributes>
    <mj-font />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new FontHelper());

            AssertHelpers.HtmlFileAsset("Font.html", result);
        }
    }
}
