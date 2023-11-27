using FluentAssertions;
using HtmlPerformanceKit;
using Mjml.Net;
using Mjml.Net.Internal;
using Xunit;

namespace Tests;

public class HtmlReaderTests
{
    [Fact]
    public void Should_read_inner_With_child()
    {
        var input = @"
<div>
    <a></a>
</div>
";
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a"))));
    }

    [Fact]
    public void Should_read_inner_With_children()
    {
        var input = @"
<div>
    <a></a>
    <a></a>
</div>
";
        var root = new Element();

        Read(new HtmlReaderWrapper(input), root);

        root.Should().BeEquivalentTo(
            new Element()
                .Add(new Element("div")
                    .Add(new Element("a"))
                    .Add(new Element("a"))));
    }

    [Fact]
    public void Should_read_inner_With_children_and_text()
    {
        var input = @"
<div>
    text1
    <a></a>
    text2
    <a></a>
    text3
</div>
";
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
        var input = @"
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
";
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
        var input = @"
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
";
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

    private static void Read(IHtmlReader reader, Element parent)
    {
        while (reader.Read())
        {
            if (reader.TokenKind == HtmlTokenKind.Tag)
            {
                var element = new Element(reader.Name);

                parent.Add(element);

                Read(reader.ReadSubtree(), element);
            }
        }
    }

    private sealed class Element
    {
        public string? Name { get; }

        public List<Element> Children { get; } = [];

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
