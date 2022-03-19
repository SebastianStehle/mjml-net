using Tests.Internal;
using Xunit;

namespace Tests
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

            AssertHelpers.HtmlFileAsset("Hero.html", result);
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

            AssertHelpers.HtmlFileAsset("HeroDivider.html", result);
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

            AssertHelpers.HtmlFileAsset("HeroDividers.html", result);
        }
    }
}
