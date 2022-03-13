using Mjml.Net;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class ImageTests
    {
        [Fact]
        public void Should_render_image()
        {
            var source = @"
 <mjml>
  <mj-body>
    <mj-image width=""300px"" src=""https://www.online-image-editor.com//styles/2014/images/example_image.png"" />
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.Image, result);
        }

        [Fact]
        public void Should_render_image_with_link()
        {
            var source = @"
 <mjml>
  <mj-body>
    <mj-image width=""300px"" src=""https://www.online-image-editor.com//styles/2014/images/example_image.png"" href=""link/to/website"" />
  </mj-body>
</mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true
            });

            TestHelpers.TrimmedContains(Resources.ImageWithLink, result);
        }
    }
}
