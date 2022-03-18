using Xunit;

namespace Tests
{
    public class GroupTests
    {
        [Fact]
        public void Should_render_groups()
        {
            var source = @"
<mjml-test head=""false"">
    <mj-group>
        <mj-spacer />
    </mj-group>
    <mj-group>
        <mj-spacer />
    </mj-group>
</mjml-test>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("Group.html"), result);
        }
    }
}
