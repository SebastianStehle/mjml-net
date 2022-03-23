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

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("41d58ca8b0b9")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.Carousel.html", result);
        }

        [Fact]
        public void Should_render_carousel_images_one()
        {
            var source = @"
<mj-carousel>
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
</mj-carousel>
";

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("a8a9d55bbf42")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselImagesOne.html", result);
        }

        [Fact]
        public void Should_render_carousel_images_two()
        {
            var source = @"
<mj-carousel>
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
</mj-carousel>
";

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("424249025dc2")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselImagesTwo.html", result);
        }

        [Fact]
        public void Should_render_carousel_images_five()
        {
            var source = @"
<mj-carousel>
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
</mj-carousel>
";

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("6123601308e5")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselImagesFive.html", result);
        }

        [Fact]
        public void Should_render_carousel_thumbnail_width()
        {
            var source = @"
<mj-carousel tb-width=""24px"">
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
    <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
</mj-carousel>
";

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("94288ecacbe4")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselThumbnailWidth.html", result);
        }

        [Fact]
        public void Should_render_carousel_head_styles()
        {
            var source = @"
<mjml-test head=""true"" body=""false"">
    <mj-head>
    </mj-head>
    <mj-body>
        <mj-carousel>
            <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/11/ecommerce-guide.jpg"" />
            <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/3@1x.png"" />
            <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/1@1x.png"" />
            <mj-carousel-image src=""https://www.mailjet.com/wp-content/uploads/2016/09/1@1x.png"" />
        </mj-carousel>
    </mj-body>
</mjml-test>
";

            var result = TestHelper.Render(source, new MjmlOptions
            {
                IdGenerator = new StaticIdGenerator("9dd9de129727")
            });

            AssertHelpers.HtmlFileAssert("Components.Outputs.CarouselHeadStyles.html", result);
        }
    }
}
