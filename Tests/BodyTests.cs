using Tests.Internal;
using Xunit;

namespace Tests
{
    public class BodyTests
    {
        [Fact]
        public void Should_add_background_to_body()
        {
            var source = @"
<mjml>
    <mj-body background-color=""red"">
    </mj-body>
</mjml>
";

            var result = TestHelper.Render(source);

            AssertHelpers.TrimmedContains(@"<body style=""background-color:red;word-spacing:normal;"">", result);
        }
    }
}
