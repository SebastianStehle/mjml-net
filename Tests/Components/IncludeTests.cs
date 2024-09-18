using Mjml.Net;
using Tests.Internal;

namespace Tests.Components;

public class IncludeTests
{
    [Fact]
    public void Should_include_css()
    {
        var files = new Dictionary<string, string>
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

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.StyleInclude.html", result);
    }

    [Fact]
    public void Should_include_mjml_fragments()
    {
        var files = new Dictionary<string, string>
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

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.Group.html", result);
    }

    [Fact]
    public void Should_include_mjml()
    {
        var files = new Dictionary<string, string>
        {
            ["./text.mjml"] = @"<mj-text>Hello MJML</mj-text>"
        };

        var source = @"
<mjml-test head=""false"">
    <mj-include path=""./text.mjml"" />
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWhitespace.html", result);
    }

    [Fact]
    public void Should_include_mjml_with_dummy_body()
    {
        var files = new Dictionary<string, string>
        {
            ["./text.mjml"] = @"
                <mjml>
                    <mj-body>
                        <mj-text>Hello MJML</mj-text>
                    </mj-body>
                </mjml>"
        };

        const string source = @"
<mjml-test head=""false"">
    <mj-text>Before Include</mj-text>
    <mj-include path=""./text.mjml"" />
    <mj-text>After Include</mj-text>
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextInclude.html", result);
    }

    [Fact]
    public void Should_include_mjml_nested()
    {
        var files = new Dictionary<string, string>
        {
            ["./headers/header.mjml"] = @"<mj-include path=""text.mjml"" />",
            ["./headers/text.mjml"] = @"<mj-text>Hello MJML</mj-text>"
        };

        var source = @"
<mjml-test head=""false"">
    <mj-include path=""./headers/header.mjml"" />
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.TextWhitespace.html", result);
    }

    [Fact]
    public void Should_include_html()
    {
        var files = new Dictionary<string, string>
        {
            ["./text.html"] = @"<strong>Hello</strong> <strong>MJML</strong"
        };

        var source = @"
<mjml-test head=""false"">
    <mj-include path=""./text.html"" type=""html"" />
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, new MjmlOptions
        {
            FileLoader = () => new InMemoryFileLoader(files)
        });

        AssertHelpers.HtmlAssert(files["./text.html"]!, result);
    }
}
