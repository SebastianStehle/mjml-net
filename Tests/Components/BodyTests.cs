using Tests.Internal;

namespace Tests.Components;

public class BodyTests
{
    [Fact]
    public void Should_render_body_only()
    {
        var source = @"
<mjml>
    <mj-body>
    </mj-body>
 </mjml>
";

        var (result, _) = TestHelper.Render(source);

        Assert.Contains("</body>", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Should_add_background_to_body()
    {
        var source = @"
<mjml>
    <mj-body background-color=""red"">
    </mj-body>
</mjml>
";

        var (result, _) = TestHelper.Render(source);

        Assert.Contains(@"<body style=""background-color:red;word-spacing:normal;"">", result, StringComparison.OrdinalIgnoreCase);
    }
}
