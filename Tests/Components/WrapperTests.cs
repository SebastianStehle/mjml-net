using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class WrapperTests
    {
        [Fact]
        public void Should_render_wrapper()
        {
            var source = @"
<mj-wrapper>
	<mj-spacer css-class=""class1"" />
    <mj-spacer css-class=""class2"" />
</mj-wrapper>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.Wrapper.html", result);
        }
    }
}
