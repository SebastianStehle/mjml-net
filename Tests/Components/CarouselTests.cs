using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class CarouselTests
    {
        [Fact]
        public void Should_render_carousel()
        {
            var source = @"
<mj-carousel>
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/3@1x.png"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/1@1x.png"" />
</mj-carousel>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Carousel.html", result);
        }

        [Fact]
        public void Should_render_carousel_with_head()
        {
            var source = @"
<mjml-test head = ""true"">
    <mj-carousel>
        <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
        <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/3@1x.png"" />
        <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/1@1x.png"" />
        <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/1@1x.png"" />
    </mj-carousel>
</mjml-test>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselWithHead.html", result);
        }
    }
}
