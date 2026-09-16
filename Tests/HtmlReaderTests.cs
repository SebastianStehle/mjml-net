using System.Text;
using FluentAssertions;
using HtmlPerformanceKit;
using Mjml.Net;
using Mjml.Net.Internal;

namespace Tests;

public class HtmlReaderTests
{
    [Fact]
    public void Should_read_inner_with_child()
    {
        var input = """
            <div>
                <a></a>
            </div>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a"))));
    }

    [Fact]
    public void Should_end_parent_when_subtree_of_self_closing_element_reads_end_tag_of_parent()
    {
        // Without whitespace, so that there are no text tokens between the tags.
        var input = "<section><column><text /></column></section><section></section>";
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("section")
                    .Add(new Element("column")
                        .Add(new Element("text"))))
                .Add(new Element("section")));
    }

    [Fact]
    public void Should_read_inner_html_without_trailing_whitespace_text()
    {
        var reader = new HtmlReaderWrapper("<div>\n<p class=\"a b\">Hello</p><!--comment-->\n\t\t</div>");
        reader.Read();

        var sb = new StringBuilder();
        reader.ReadInnerHtml().AppendTo(sb);

        Assert.Equal("<p class=\"a b\">Hello</p><!-- comment -->", sb.ToString());
    }

    [Fact]
    public void Should_read_empty_inner_html()
    {
        var reader = new HtmlReaderWrapper("<div>\n\t\t</div>");
        reader.Read();

        var inner = reader.ReadInnerHtml();

        Assert.True(inner.IsEmpty());
    }

    [Fact]
    public void Should_read_inner_with_children()
    {
        var input = """
            <div>
                <a></a>
                <a></a>
            </div>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a"))
                    .Add(new Element("a"))));
    }

    [Fact]
    public void Should_read_inner_with_children_and_text()
    {
        var input = """
            <div>
                text1
                <a></a>
                text2
                <a></a>
                text3
            </div>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a"))
                    .Add(new Element("a"))));
    }

    [Fact]
    public void Should_read_nested()
    {
        var input = """
            <div>
                text1
                <a>
                    <strong></strong>
                    inner
                </a>
                <a>
                    <strong></strong>
                    inner
                </a>
                text2
            </div>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a")
                        .Add(new Element("strong")))
                    .Add(new Element("a")
                        .Add(new Element("strong")))));
    }

    [Fact]
    public void Should_read_invalid_html()
    {
        var input = """
            <div>
                text1
                <a>
                    <br></br>
                </a>
                <a>
                    <strong></strong>
                    inner
                </a>
                text2
            </div>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a")
                        .Add(new Element("br")))
                    .Add(new Element("a")
                        .Add(new Element("strong")))));
    }
    [Fact]
    public void Should_read_single_quoted_with_ambersand()
    {
        var input = """
            <a href='&'></a>
            <a href="url"></a>
            """;
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("a")
                {
                    Attributes = new Dictionary<string, string>
                    {
                        ["href"] = "&"
                    }
                })
                .Add(new Element("a")
                {
                    Attributes = new Dictionary<string, string>
                    {
                        ["href"] = "url"
                    }
                }));
    }

    private static void Read(IHtmlReader reader, Element parent)
    {
        while (reader.Read())
        {
            if (reader.TokenKind == HtmlTokenKind.Tag)
            {
                var element = new Element(reader.Name);

                if (reader.AttributeCount > 0)
                {
                    element.Attributes = [];

                    for (var i = 0; i < reader.AttributeCount; i++)
                    {
                        element.Attributes[reader.GetAttributeName(i)] = reader.GetAttribute(i);
                    }
                }

                parent.Add(element);

                Read(reader.ReadSubtree(), element);
            }
        }
    }

    private sealed class Element
    {
        public string? Name { get; }

        public List<Element> Children { get; } = [];

        public Dictionary<string, string>? Attributes { get; set; }

        public Element(string? name = null)
        {
            Name = name;
        }

        public Element Add(Element element)
        {
            Children.Add(element);
            return this;
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
