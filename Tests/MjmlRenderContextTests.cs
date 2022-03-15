using System;
using System.Text;
using Mjml.Net;
using Xunit;

namespace Tests
{
    public class MjmlRenderContextTests
    {
        private readonly MjmlRenderContext sut = new MjmlRenderContext(new MjmlRenderer(), new MjmlOptions
        {
            Beautify = true
        });

        public MjmlRenderContextTests()
        {
            sut.BufferStart();
        }

        [Fact]
        public void Should_render_element_on_flush()
        {
            sut.ElementStart("div");

            AssertText(new[]
            {
                "<div>"
            });
        }

        [Fact]
        public void Should_render_element_when_closed()
        {
            sut.ElementStart("div");
            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div>",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_element_with_attribute()
        {
            sut.ElementStart("div")
                .Attr("attr1", "1")
                .Attr("attr2", "2")
                .Attr("attr3", null);

            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div attr1='1' attr2='2'>",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_element_with_styles()
        {
            sut.ElementStart("div")
                .Style("style1", "1")
                .Style("style2", "2")
                .Style("style3", null);

            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div style='style1:1;style2:2;'>",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_element_with_classes()
        {
            sut.ElementStart("div")
                .Class("class1")
                .Class("class2")
                .Class(null);

            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div class='class1 class2'>",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_nested_elements()
        {
            sut.ElementStart("div");

            sut.ElementStart("div1");
            sut.ElementEnd("div1");

            sut.ElementStart("div2");

            sut.ElementStart("div2_1");
            sut.ElementEnd("div2_1");

            sut.ElementStart("div2_2");
            sut.ElementEnd("div2_2");

            sut.ElementEnd("div2");

            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div>",
                "  <div1>",
                "  </div1>",
                "  <div2>",
                "    <div2_1>",
                "    </div2_1>",
                "    <div2_2>",
                "    </div2_2>",
                "  </div2>",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_elements_with_content()
        {
            sut.ElementStart("div");
            sut.Content("1");
            sut.Content("2");
            sut.Content(null);
            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div>",
                "  1",
                "  2",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_elements_with_multiliner_content()
        {
            sut.ElementStart("div");
            sut.Content($"line1{Environment.NewLine}line2{Environment.NewLine}");
            sut.Content("line3");
            sut.ElementEnd("div");

            AssertText(new[]
            {
                "<div>",
                "  line1",
                "  line2",
                "  ",
                "  line3",
                "</div>"
            });
        }

        [Fact]
        public void Should_render_elements_with_plain_text()
        {
            sut.Plain("before");
            sut.ElementStart("div");
            sut.Plain("1");
            sut.Plain("2");
            sut.ElementEnd("div");
            sut.Plain("after");

            AssertText(new[]
            {
                "before",
                "<div>",
                "1",
                "2",
                "</div>",
                "after"
            });
        }

        [Fact]
        public void Should_render_buffered()
        {
            sut.ElementStart("html");

            sut.BufferStart();
            sut.ElementStart("head");
            sut.Content("head");
            sut.ElementEnd("head");

            var head = sut.BufferFlush();

            sut.BufferStart();
            sut.ElementStart("body");
            sut.Content("body");
            sut.ElementEnd("body");

            var body = sut.BufferFlush();

            sut.Plain(head, false);
            sut.Plain(body, false);

            sut.ElementEnd("html");

            AssertText(new[]
            {
                "<html>",
                "  <head>",
                "    head",
                "  </head>",
                "  <body>",
                "    body",
                "  </body>",
                "</html>"
            });
        }

        private void AssertText(params string[] lines)
        {
            var sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line.Replace('\'', '"'));
            }

            var actual = sut.BufferFlush();

            Assert.Equal(sb.ToString(), actual);
        }
    }
}
