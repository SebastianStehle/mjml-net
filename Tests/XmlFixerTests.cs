using Mjml.Net;
using System.Xml.Linq;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class XmlFixerTests
    {
        [Fact]
        public void Should_convert_self_closing_tag()
        {
            var input = @"
<div>
    <br>
</div>";

            var expected = @"
<div>
    <br />
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_self_closing_tag2()
        {
            var input = @"
<div>
    <br></br>
</div>";

            var expected = @"
<div>
    <br></br>
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_less_than_symbol()
        {
            var input = @"
<div>
    <
</div>";

            var expected = @"
<div>
    &lt;
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_less_than_symbol_in_text()
        {
            var input = @"
<div>
    Hello < World
</div>";

            var expected = @"
<div>
    Hello &lt; World
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_greater_than_symbol()
        {
            var input = @"
<div>
    >
</div>";

            var expected = @"
<div>
    &gt;
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_greater_than_symbol_in_text()
        {
            var input = @"
<div>
    Hello > World
</div>";

            var expected = @"
<div>
    Hello &gt; World
</div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_convert_ambersand_in_attribute()
        {
            var input = @"
<div name=""&""></div>";

            var expected = @"
<div name=""&amp;""></div>";

            TestHtml(expected, input);
        }

        [Fact]
        public void Should_not_convert_entity_in_attribute()
        {
            var input = @"
<div name=""&lt;""></div>";

            var expected = @"
<div name=""&lt;""></div>";

            TestHtml(expected, input);
        }

        private static void TestHtml(string expected, string input)
        {
            input = input.Replace("\r\n", "\n").Trim();

            var actual = XmlFixer.Process2(input);

            expected = expected.Replace("\r\n", "\n").Trim();

            Assert.Equal(expected, actual);
            AssertHtml(actual);

            static void AssertHtml(string text)
            {
                try
                {
                    XDocument.Parse(text);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Failed to parse XML with {ex.Message}.");
                }
            }
        }
    }
}
