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

        [Fact]
        public void Should_not_cause_issues_with_entity_expansion()
        {
            var mjml = @"
<mjml>
    <mj-head></mj-head>
    <mj-body>
        <mj-text><span>&lol9;</span></mj-text>
    </mj-body>
</mjml>
";

            var options = new MjmlOptions
            {
                XmlEntities = new Dictionary<string, string>
                {
                    ["lol"] = "lol",
                    ["lol2"] = "&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;&lol;",
                    ["lol3"] = "&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;&lol2;",
                    ["lol4"] = "&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;&lol3;",
                    ["lol5"] = "&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;&lol4;",
                    ["lol6"] = "&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;&lol5;",
                    ["lol7"] = "&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;&lol6;",
                    ["lol8"] = "&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;&lol7;",
                    ["lol9"] = "&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;&lol8;"
                }
            };

            var sut = new MjmlRenderer();

            var result = sut.Render(mjml);

            Assert.Contains("<span>&lol9;</span>", result.Html!, StringComparison.OrdinalIgnoreCase);
        }
    }
}
