using Mjml.Net.Helpers;
using Tests.Internal;

namespace Tests.Components;

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

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

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

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.FontUbuntu.html", result);
    }

    [Fact]
    public void Should_add_font_implicitely_but_not_override_custom()
    {
        var source = @"
<mjml-test body=""false"">
    <mj-head>
        <mj-font name=""Ubuntu"" href=""https://fonts.googleapis.com/css?family=Ubuntu:300,400"" />
    </mj-head>
    <mj-body>
        <mj-text font-family=""Ubuntu""></mj-text>
    </mj-body>
</mjml-test>
";

        var (result, _) = TestHelper.Render(source, helpers: [new FontHelper()]);

        AssertHelpers.HtmlFileAssert("Components.Outputs.FontUbuntu2.html", result);
    }
}
