using Mjml.Net;
using Tests.Internal;

namespace Tests;

public class HtmlSpecialCaseTests
{
    [Fact]
    public void Should_ignore_extra_elements()
    {
        var source = """
            <mj-html-attributes>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="content-background-color" content-background-color="#ffffff"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="text-color" text-color="#000000"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="font-family" font-family="-apple-system, BlinkMacSystemFont, &#x27;Segoe UI&#x27;, &#x27;Roboto&#x27;, &#x27;Oxygen&#x27;, &#x27;Ubuntu&#x27;, &#x27;Cantarell&#x27;, &#x27;Fira Sans&#x27;, &#x27;Droid Sans&#x27;,&#x27;Helvetica Neue&#x27;, sans-serif"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="font-size" font-size="12px"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="line-height" line-height="1.7"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="font-weight" font-weight="400"></mj-html-attribute>
                  <mj-html-attribute class="easy-email" multiple-attributes="false" attribute-name="responsive" responsive="true"></mj-html-attribute>
            </mj-html-attributes>

            <mj-button font-family="Helvetica" background-color="#f45e43" color="white">
                Button
            </mj-button>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Button.html", result);
    }

    [Fact]
    public void Should_expose_html_errors()
    {
        var source = $@"
<>
";

        var result = TestHelper.Render(source);

        Assert.Contains(
            new ValidationError(
                "Unexpected character in stream.",
                ValidationErrorType.InvalidHtml,
                new SourcePosition(2, 3, null)),
            result.Errors);
    }
}
