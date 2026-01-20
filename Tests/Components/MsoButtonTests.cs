using Tests.Internal;

namespace Tests.Components;

public class MsoButtonTests
{
    [Fact]
    public void Should_render_regular_button_by_default()
    {
        var source = """
            <mj-msobutton font-family="Helvetica" background-color="#f45e43" color="white">
                Button
            </mj-msobutton>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Button.html", result);
    }

    [Fact]
    public void Should_render_mso_proof_button()
    {
        var source = """
            <mj-msobutton mso-proof="true" font-family="Helvetica" background-color="#f45e43" color="white">
                Button
            </mj-msobutton>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.MsoButton.html", result);
    }

    [Fact]
    public void Should_render_mso_proof_button_with_border()
    {
        var source = """
            <mj-msobutton mso-proof="true" height="48px" width="252px" border="2px dashed #1f2153" background-color="none"  border-radius="22px" color="black">
                Reset Password
            </mj-msobutton>
            """;

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.MsoButtonWithBorder.html", result);
    }
}
