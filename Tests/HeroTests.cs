using Tests.Properties;
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

            AssertHelpers.HtmlAssert(Resources.Hero, result);
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

            AssertHelpers.HtmlAssert(Resources.Image, result);
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

            AssertHelpers.HtmlAssert(Resources.Image, result);
        }

        [Fact]
        public void Should_render_hero_with_raw()
        {
            var source = @"
<mj-hero>
  <mj-raw>
    Hello MJML
  </mj-raw>
</mj-hero>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(Resources.Image, result);
        }
    }
}
