using Mjml.Net;
using Xunit;

namespace Tests
{
    public class SecurityTests
    {
        [Fact]
        public void Should_not_allow_custom_dtd()
        {
            var mjml = @"
<!DOCTYPE mjml>
<mjml>
    <mj-head></mj-head>
    <mj-body></mj-body>
</mjml>
";

            var sut = new MjmlRenderer();

            Assert.ThrowsAny<Exception>(() => sut.Render(mjml));
        }
    }
}
