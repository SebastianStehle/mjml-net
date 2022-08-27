using Mjml.Net;
using Mjml.Net.Helpers;
using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class IncludeTests
    {
        [Fact]
        public void Should_include_css()
        {
            var files = new InMemoryFileLoader
            {
                ["./style.css"] = @"
.red-text div {
  color: red !important;
}"
            };

            var source = @"
<mjml-test body=""false"">
  <mj-head>
    <mj-include path=""./style.css"" type=""css"" />
  </mj-head>
  <mj-body>
  </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new MjmlOptions { FileLoader = files }, new StyleHelper());

            AssertHelpers.HtmlFileAssert("Components.Outputs.Style.html", result);
        }

        [Fact]
        public void Should_include_mjml_fragments()
        {
            var files = new InMemoryFileLoader
            {
                ["./text.mjml"] = @"
<mj-group>
    <mj-spacer />
</mj-group>
<mj-group>
    <mj-spacer />
</mj-group>"
            };

            var source = @"
<mjml-test head=""false"">
    <mj-include path=""./text.mjml"" />
</mjml-test>
";

            var result = TestHelper.Render(source, new MjmlOptions { FileLoader = files });

            AssertHelpers.HtmlFileAssert("Components.Outputs.Group.html", result);
        }

        [Fact]
        public void Should_include_mjml()
        {
            var files = new InMemoryFileLoader
            {
                ["./text.mjml"] = @"<mj-text>Hello MJML</mj-text>"
            };

            var source = @"
<mjml-test head=""false"">
    <mj-include path=""./text.mjml"" />
</mjml-test>
";

            var result = TestHelper.Render(source, new MjmlOptions { FileLoader = files });

            AssertHelpers.HtmlFileAssert("Components.Outputs.TextWhitespace.html", result);
        }

        [Fact]
        public void Should_include_html()
        {
            var files = new InMemoryFileLoader
            {
                ["./text.html"] = @"<strong>Hello</strong> <strong>MJML</strong"
            };

            var source = @"
<mjml-test head=""false"">
    <mj-include path=""./text.html"" type=""html"" />
</mjml-test>
";

            var result = TestHelper.Render(source, new MjmlOptions { FileLoader = files });

            AssertHelpers.HtmlAssert(files["./text.html"]!, result);
        }
    }
}
