using Tests.Internal;
using Xunit;

namespace Tests.Components
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

            AssertHelpers.HtmlFileAsset("Components.Outputs.Group.html", result);
        }

        [Fact]
        public void Should_render_group_with_columns()
        {
            var source = @"
<mjml-test head=""false"">
    <mj-group>
        <mj-column width=""100px""></mj-column>
        <mj-column></mj-column>
    </mj-group>
</mjml-test>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.GroupWithColumns.html", result);
        }
    }
}
