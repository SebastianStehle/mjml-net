using System.Text;
using Mjml.Net;
using Mjml.Net.Extensions;
using Xunit;

namespace Tests
{
    public class HtmlExtensionsTests
    {
        private readonly MjmlRenderContext sut = new MjmlRenderContext(new MjmlRenderer(), new MjmlOptions
        {
            Beautify = true
        });

        public HtmlExtensionsTests()
        {
            sut.StartBuffer();
        }

        [Fact]
        public void Should_suffix_single_class()
        {
            sut.StartElement("div")
                .Classes("class1", "outlook");

            AssertText("<div class=\"class1-outlook\">");
        }

        [Fact]
        public void Should_suffix_multiple_classes()
        {
            sut.StartElement("div")
                .Classes("class1 class2", "outlook");

            AssertText("<div class=\"class1-outlook class2-outlook\">");
        }

        [Fact]
        public void Should_suffix_multiple_classes2()
        {
            sut.StartElement("div")
                .Classes("class1  class2", "outlook");

            AssertText("<div class=\"class1-outlook class2-outlook\">");
        }

        [Fact]
        public void Should_suffix_multiple_classes3()
        {
            sut.StartElement("div")
                .Classes(" class1 class2 ", "outlook");

            AssertText("<div class=\"class1-outlook class2-outlook\">");
        }

        [Fact]
        public void Should_suffix_no_classes()
        {
            sut.StartElement("div")
                .Classes(string.Empty, "outlook");

            AssertText("<div>");
        }

        [Fact]
        public void Should_suffix_no_suffix()
        {
            sut.StartElement("div")
                .Classes("class1 class2", string.Empty);

            AssertText("<div class=\"class1 class2\">");
        }

        private void AssertText(params string[] lines)
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.Replace('\'', '"'));
            }

            var actual = sut.EndBuffer()!.ToString();

            Assert.Equal(sb.ToString(), actual);
        }
    }
}
