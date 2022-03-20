using Tests.Internal;
using Xunit;

namespace Tests
{
    public class NavbarTests
    {
        [Fact]
        public void Should_render_empty_navbar()
        {
            var source = @"<mj-navbar hamburger=""hamburger""></mj-navbar>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("Navbar.html", result);
        }

        [Fact]
        public void Should_render_empty_navbar_without_hamburger()
        {
            var source = @"<mj-navbar></mj-navbar>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("NavbarWithoutHamburger.html", result);
        }
    }
}
