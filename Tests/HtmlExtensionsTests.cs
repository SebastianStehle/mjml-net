using Mjml.Net;
using Mjml.Net.Extensions;
using Tests.Internal;
using Xunit;

namespace Tests;

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

        AssertHelpers.MultilineText(sut,
            @"<div class=""class1-outlook"">"
        );
    }

    [Fact]
    public void Should_suffix_multiple_classes()
    {
        sut.StartElement("div")
            .Classes("class1 class2", "outlook");

        AssertHelpers.MultilineText(sut,
            @"<div class=""class1-outlook class2-outlook"">"
        );
    }

    [Fact]
    public void Should_suffix_multiple_classes2()
    {
        sut.StartElement("div")
            .Classes("class1  class2", "outlook");

        AssertHelpers.MultilineText(sut,
            @"<div class=""class1-outlook class2-outlook"">");
    }

    [Fact]
    public void Should_suffix_multiple_classes3()
    {
        sut.StartElement("div")
            .Classes(" class1 class2 ", "outlook");

        AssertHelpers.MultilineText(sut,
            @"<div class=""class1-outlook class2-outlook"">"
        );
    }

    [Fact]
    public void Should_suffix_no_classes()
    {
        sut.StartElement("div")
            .Classes(string.Empty, "outlook");

        AssertHelpers.MultilineText(sut,
            "<div>"
        );
    }

    [Fact]
    public void Should_suffix_no_suffix()
    {
        sut.StartElement("div")
            .Classes("class1 class2", string.Empty);

        AssertHelpers.MultilineText(sut,
            @"<div class=""class1 class2"">"
        );
    }
}
