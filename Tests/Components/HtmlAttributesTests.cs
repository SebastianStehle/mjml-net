using Mjml.Net;
using Tests.Internal;

namespace Tests.Components;

public class HtmlAttributesTests
{
    [Fact]
    public async Task Should_render_attributes()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-html-attributes>
      <mj-selector path="".custom div"">
        <mj-html-attribute name=""data-id"">42</mj-html-attribute>
      </mj-selector>
    </mj-html-attributes>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors = [AngleSharpPostProcessor.Default]
        }, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributes.html", result);
    }

    [Fact]
    public async Task Should_render_selector_only()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-selector path="".custom div"">
      <mj-html-attribute name=""data-id"">42</mj-html-attribute>
    </mj-selector>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors = [AngleSharpPostProcessor.Default]
        }, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributeInvalid.html", result);
    }

    [Fact]
    public async Task Should_render_attribute_only()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-html-attribute name=""data-id"">42</mj-html-attribute>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, new MjmlOptions
        {
            PostProcessors = [AngleSharpPostProcessor.Default]
        }, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributeInvalid.html", result);
    }

    [Fact]
    public async Task Should_not_render_attributes_without_processor()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-html-attributes>
      <mj-selector path="".custom div"">
        <mj-html-attribute name=""data-id"">42</mj-html-attribute>
      </mj-selector>
    </mj-html-attributes>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributesNoProcessor.html", result);
    }

    [Fact]
    public async Task Should_not_render_selector_without_processor()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-selector path="".custom div"">
      <mj-html-attribute name=""data-id"">42</mj-html-attribute>
    </mj-selector>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributesNoProcessor.html", result);
    }

    [Fact]
    public async Task Should_not_render_attribute_without_processor()
    {
        var source = @"
<mjml-test>
  <mj-head>
    <mj-html-attribute name=""data-id"">42</mj-html-attribute>
  </mj-head>
  <mj-body>
    <mj-raw>
      <div class=""custom"">
        <div></div>
      </div>
    </mj-raw>
  </mj-body>
</mjml-test>
";

        var (result, _) = await TestHelper.RenderAsync(source, helpers: []);

        AssertHelpers.HtmlFileAssert("Components.Outputs.HtmlAttributesNoProcessor.html", result);
    }
}
