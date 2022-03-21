using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class HeroTests
    {
        [Fact]
        public void Should_render_hero()
        {
            var source = @"
<mj-hero>
</mj-hero>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.Hero.html", result);
        }

        [Fact]
        public void Should_render_hero_with_child()
        {
            var source = @"
<mj-hero>
  <mj-divider />
</mj-hero>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.HeroDivider.html", result);
        }

        [Fact]
        public void Should_render_hero_with_children()
        {
            var source = @"
<mj-hero>
  <mj-divider />
  <mj-divider />
</mj-hero>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Components.Outputs.HeroDividers.html", result);
        }
    }
}
