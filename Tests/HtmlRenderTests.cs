using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests
{
    public class HtmlRenderTests
    {
        private readonly MjmlRenderContext sut = new MjmlRenderContext(new MjmlRenderer(), new MjmlOptions
        {
            Beautify = true
        });

        public HtmlRenderTests()
        {
            sut.StartBuffer();
        }

        public static IEnumerable<object[]> Cultures()
        {
            yield return new object[] { "en-US" };
            yield return new object[] { "de-DE" };
            yield return new object[] { "es-ES" };
            yield return new object[] { string.Empty };
        }

        [Fact]
        public void Should_render_element_on_flush()
        {
            sut.StartElement("div");

            AssertHelpers.MultilineText(sut,
                "<div>"
            );
        }

        [Fact]
        public void Should_render_element_when_closed()
        {
            sut.StartElement("div");
            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div>",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_element_with_attribute()
        {
            sut.StartElement("div")
                .Attr("attr1", "1")
                .Attr("attr2", "2")
                .Attr("attr3", $"{3}")
                .Attr("attr4", null);

            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div attr1='1' attr2='2' attr3='3'>",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_element_with_styles()
        {
            sut.StartElement("div")
                .Style("style1", "1")
                .Style("style2", "2")
                .Style("style3", $"{3}")
                .Style("style4", null);

            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div style='style1:1;style2:2;style3:3;'>",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_element_with_classes()
        {
            sut.StartElement("div")
                .Class("class1")
                .Class("class2")
                .Class($"class{3}")
                .Class(null);

            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div class='class1 class2 class3'>",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_nested_elements()
        {
            sut.StartElement("div");

            sut.StartElement("div1");
            sut.EndElement("div1");

            sut.StartElement("div2");

            sut.StartElement("div2_1");
            sut.EndElement("div2_1");

            sut.StartElement("div2_2");
            sut.EndElement("div2_2");

            sut.EndElement("div2");

            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
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
            );
        }

        [Fact]
        public void Should_render_elements_with_content()
        {
            sut.StartElement("div");
            sut.Content("1");
            sut.Content("2");
            sut.Content(null);
            sut.Content($"{3}");
            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div>",
                "  1",
                "  2",
                "  3",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_elements_with_multiliner_content()
        {
            var line12 = $"line1{Environment.NewLine}line2{Environment.NewLine}";
            var line3 = "line3";

            sut.StartElement("div");
            sut.Content(line12);
            sut.Content(line3);
            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div>",
                "  line1",
                "  line2",
                "  ",
                "  line3",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_elements_with_plain_text()
        {
            sut.Plain("before");
            sut.StartElement("div");
            sut.Plain("1");
            sut.Plain("2");
            sut.EndElement("div");
            sut.Plain("after");

            AssertHelpers.MultilineText(sut,
                "before",
                "<div>",
                "1",
                "2",
                "</div>",
                "after"
           );
        }

        [Fact]
        public void Should_render_buffered()
        {
            sut.StartElement("html");

            sut.StartBuffer();
            sut.StartElement("head");
            sut.Content("head");
            sut.EndElement("head");

            var head = sut.EndBuffer();

            sut.StartBuffer();
            sut.StartElement("body");
            sut.Content("body");
            sut.EndElement("body");

            var body = sut.EndBuffer();

            sut.Plain(head, false);
            sut.Plain(body, false);

            sut.EndElement("html");

            AssertHelpers.MultilineText(sut,
                "<html>",
                "  <head>",
                "    head",
                "  </head>",
                "  <body>",
                "    body",
                "  </body>",
                "</html>"
            );
        }

        [Fact]
        public void Should_not_render_nested_conditional()
        {
            sut.StartConditional("<-- open -->");
            sut.StartConditional("<-- open -->");
            sut.EndConditional("<-- close -->");
            sut.EndConditional("<-- close -->");

            AssertHelpers.MultilineText(sut,
                "<-- open -->",
                "<-- close -->"
            );
        }

        [Fact]
        public void Should_not_render_nested_conditional2()
        {
            sut.StartConditional("<-- open -->");
            sut.EndConditional("<-- close -->");
            sut.StartConditional("<-- open -->");
            sut.EndConditional("<-- close -->");

            AssertHelpers.MultilineText(sut,
                "<-- open -->",
                "<-- close -->"
            );
        }

        [Fact]
        public void Should_render_conditionals()
        {
            sut.StartConditional("<-- open -->");
            sut.StartElement("div");

            sut.EndElement("div");
            sut.EndConditional("<-- close -->");

            AssertHelpers.MultilineText(sut,
                "<-- open -->",
                "<div>",
                "</div>",
                "<-- close -->"
            );
        }

        [Fact]
        public void Should_render_conditionals2()
        {
            sut.StartElement("div");
            sut.StartConditional("<-- open -->");
            sut.StartElement("div");

            sut.EndElement("div");
            sut.EndConditional("<-- close -->");
            sut.EndElement("div");

            AssertHelpers.MultilineText(sut,
                "<div>",
                "  <-- open -->",
                "  <div>",
                "  </div>",
                "  <-- close -->",
                "</div>"
            );
        }

        [Fact]
        public void Should_render_conditionals3()
        {
            sut.StartConditional("<-- open -->");
            sut.StartElement("div", true);
            sut.EndConditional("<-- close -->");

            sut.StartConditional("<-- open -->");
            sut.StartElement("div", true);
            sut.EndConditional("<-- close -->");

            AssertHelpers.MultilineText(sut,
                "<-- open -->",
                "<div/>",
                "<div/>",
                "<-- close -->"
            );
        }

        [Fact]
        public void Should_render_conditionals_self_closed()
        {
            sut.StartConditional("<-- open -->");
            sut.StartElement("div", true);
            sut.EndConditional("<-- close -->");

            AssertHelpers.MultilineText(sut,
                "<-- open -->",
                "<div/>",
                "<-- close -->"
            );
        }

        [Theory]
        [MemberData(nameof(Cultures))]
        public void Should_render_interolated_content_with_invariant_culture(string culture)
        {
            TestHelper.TestWithCulture(culture, () =>
            {
                sut.Content(@$"<div class=""{0.3333}px"">");

                AssertHelpers.MultilineText(sut,
                    @"<div class=""0.3333px"">"
                );
            });
        }

        [Theory]
        [MemberData(nameof(Cultures))]
        public void Should_render_interolated_class_with_invariant_culture(string culture)
        {
            TestHelper.TestWithCulture(culture, () =>
            {
                sut.StartElement("div")
                    .Class($"{0.3333}px");

                AssertHelpers.MultilineText(sut,
                    @"<div class=""0.3333px"">"
                );
            });
        }

        [Theory]
        [MemberData(nameof(Cultures))]
        public void Should_render_interolated_attribute_with_invariant_culture(string culture)
        {
            TestHelper.TestWithCulture(culture, () =>
            {
                sut.StartElement("div")
                    .Attr("width", $"{0.3333}px");

                AssertHelpers.MultilineText(sut,
                    @"<div width=""0.3333px"">"
                );
            });
        }

        [Theory]
        [MemberData(nameof(Cultures))]
        public void Should_render_interolated_style_with_invariant_culture(string culture)
        {
            TestHelper.TestWithCulture(culture, () =>
            {
                sut.StartElement("div")
                    .Style("width", $"{0.3333}px");

                AssertHelpers.MultilineText(sut,
                    @"<div style=""width:0.3333px;"">"
                );
            });
        }
    }
}
