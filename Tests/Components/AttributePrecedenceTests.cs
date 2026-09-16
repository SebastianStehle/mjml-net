using System.Text.RegularExpressions;
using Mjml.Net;

namespace Tests.Components;

public partial class AttributePrecedenceTests
{
    [Fact]
    public void Should_prefer_type_attributes_over_all_attributes()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-all color="red" />
                  <mj-text color="blue" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-text>Text</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("blue", colors);
    }

    [Fact]
    public void Should_prefer_class_attributes_over_type_attributes()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-text color="blue" />
                  <mj-class name="green" color="green" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-text mj-class="green">Text</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("green", colors);
    }

    [Fact]
    public void Should_prefer_last_class()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-class name="green" color="green" />
                  <mj-class name="purple" color="purple" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-text mj-class="purple green">Text</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("green", colors);
    }

    [Fact]
    public void Should_prefer_parent_class_attributes_over_type_attributes_and_class_attributes_over_parent_class_attributes()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-text color="blue" />
                  <mj-class name="parent">
                    <mj-text color="orange" />
                  </mj-class>
                  <mj-class name="green" color="green" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column mj-class="parent">
                    <mj-text>Text1</mj-text>
                    <mj-text mj-class="green">Text2</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("orange,green", colors);
    }

    [Fact]
    public void Should_prefer_local_attributes_over_class_attributes()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-class name="green" color="green" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-text mj-class="green" color="black">Text</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("black", colors);
    }

    [Fact]
    public void Should_prefer_inherited_attributes_over_class_attributes_and_local_attributes_over_inherited_attributes()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-class name="green" color="green" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-social color="red">
                      <mj-social-element name="facebook" mj-class="green">Text1</mj-social-element>
                      <mj-social-element name="facebook" color="black">Text2</mj-social-element>
                    </mj-social>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var colors = RenderColors(source);

        Assert.Equal("red,black", colors);
    }

    [Fact]
    public void Should_not_override_side_from_class_with_shorthand()
    {
        var source = """
            <mjml>
              <mj-head>
                <mj-attributes>
                  <mj-all padding="1px 2px" />
                  <mj-class name="left" padding-left="9px" />
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-section>
                  <mj-column>
                    <mj-text mj-class="left">Text</mj-text>
                  </mj-column>
                </mj-section>
              </mj-body>
            </mjml>
            """;

        var html = new MjmlRenderer().Render(source, new MjmlOptions { Beautify = false }).Html;

        Assert.Contains("padding-left:9px;", html, StringComparison.Ordinal);
    }

    private static string RenderColors(string source)
    {
        var html = new MjmlRenderer().Render(source, new MjmlOptions { Beautify = false }).Html;

        return string.Join(",", ColorRegex().Matches(html).Select(x => x.Groups[1].Value).Where(x => x != "transparent"));
    }

    [GeneratedRegex("(?<![-a-z])color:([a-z]+)")]
    private static partial Regex ColorRegex();
}
