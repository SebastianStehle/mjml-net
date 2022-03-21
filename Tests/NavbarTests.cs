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

        [Fact]
        public void Should_render_navbar()
        {
            var source = @"
<mj-navbar base-url=""https://mjml.io"" hamburger=""hamburger"" ico-color=""black"">
    <mj-navbar-link href=""/link1"" color=""#ff00ff"">Link1</mj-navbar-link>
    <mj-navbar-link href=""/link2"" color=""#ff0000"">Link2</mj-navbar-link>
    <mj-navbar-link href=""/link3"" color=""#0000ff"">Link3</mj-navbar-link>
</mj-navbar>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAsset("NavbarWithLinks.html", result);
        }
    }
}
