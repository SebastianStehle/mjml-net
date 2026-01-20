using Mjml.Net.Helpers;
using Tests.Internal;

namespace Tests.Components;

public class AttributesTests
{
    [Fact]
    public void Should_render_font_with_attributes()
    {
        var source = """
            <mjml-test body="false">
                <mj-head>
                    <mj-attributes>
                        <mj-font name="Raleway" href="https://fonts.googleapis.com/css?family=Raleway" />
                    </mj-attributes>
                    <mj-font />
                </mj-head>
                <mj-body>
                </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
    }

    [Fact]
    public void Should_render_font_with_non_self_closing_attributes()
    {
        var source = """
            <mjml-test body="false">
                <mj-head>
                    <mj-attributes>
                        <mj-font name="Raleway" href="https://fonts.googleapis.com/css?family=Raleway"></mj-font>
                    </mj-attributes>
                    <mj-font />
                </mj-head>
                <mj-body>
                </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
    }

    [Fact]
    public void Should_render_font_with_class_attribute()
    {
        var source = """
            <mjml-test body="false">
              <mj-head>
                <mj-attributes>
                  <mj-class name="blue" href="https://fonts.googleapis.com/css?family=Raleway" />
                </mj-attributes>
                <mj-font mj-class="red blue" />
              </mj-head>
              <mj-body>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
    }

    [Fact]
    public void Should_render_font_with_last_class_attribute()
    {
        var source = """
            <mjml-test body="false">
              <mj-head>
                <mj-attributes>
                  <mj-class name="red" href="https://fonts.googleapis.com/css?family=Mono" />
                  <mj-class name="blue" href="https://fonts.googleapis.com/css?family=Raleway" />
                </mj-attributes>
                <mj-font mj-class="red blue" />
              </mj-head>
              <mj-body>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
    }

    [Fact]
    public void Should_render_font_with_multiple_attributes()
    {
        var source = """
            <mjml-test body="false">
              <mj-head>
                <mj-attributes>
                  <mj-other />
                </mj-attributes>
                <mj-attributes>
                  <mj-font name="Mono" href="https://fonts.googleapis.com/css?family=Mono" />
                </mj-attributes>
                <mj-attributes>
                  <mj-font name="Raleway" href="https://fonts.googleapis.com/css?family=Raleway" />
                </mj-attributes>
                <mj-font />
              </mj-head>
              <mj-body>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Font.html", result);
    }

    [Fact]
    public void Should_render_nested_child_class_names()
    {
        var source = """
            <mjml-test head="false">
              <mj-head>
                <mj-attributes>
                  <mj-class name="mjexampleclass" align="center" icon-size="200px">
                    <mj-social-element padding="0 10px 0 0" alt="christmas tree" src="https://cdn.pixabay.com/photo/2023/12/14/20/24/christmas-balls-8449615_1280.jpg" />
                  </mj-class>
                </mj-attributes>
              </mj-head>
              <mj-body>
                <mj-social mj-class="mjexampleclass">
                  <mj-social-element href="#">This image is not displayed when complied with mjml-net</mj-social-element>
                </mj-social>
              </mj-body>
            </mjml-test>
            """;

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ChildClasses.html", result);
    }
}
