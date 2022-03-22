using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class ImageTests
    {
        [Fact]
        public void Should_render_image()
        {
            var source = @"<mj-image width=""300px"" src=""https://www.online-image-editor.com//styles/2014/images/example_image.png"" />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Image.html", result);
        }

        [Fact]
        public void Should_render_image_with_link()
        {
            var source = @"<mj-image width=""300px"" src=""https://www.online-image-editor.com//styles/2014/images/example_image.png"" href=""link/to/website"" />";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.ImageWithLink.html", result);
        }
    }
}
