using System.Text;
using Mjml.Net;

namespace Tests;

public class InnerTextOrHtmlTests
{
    [Fact]
    public void Should_append()
    {
        var sut = new InnerTextOrHtml();

        sut.Add("a");
        sut.Add("b");
        sut.Add("c");

        var sb = new StringBuilder();
        sut.AppendTo(sb);

        Assert.Equal("abc", sb.ToString());
    }

    [Fact]
    public void Should_append_trimmed()
    {
        var sut = new InnerTextOrHtml();

        sut.Add(" \n\rStart-");
        sut.Add("a");
        sut.Add("b");
        sut.Add("c");
        sut.Add("-End\n\r ");

        var sb = new StringBuilder();
        sut.AppendTo(sb);

        Assert.Equal("Start-abc-End", sb.ToString());
    }

    [Fact]
    public void Should_not_trim_in_between()
    {
        var sut = new InnerTextOrHtml();

        sut.Add(" \n\rStart-");
        sut.Add("a");
        sut.Add(" \n\r");
        sut.Add("b");
        sut.Add(" \n\r");
        sut.Add("c");
        sut.Add("-End\n\r ");

        var sb = new StringBuilder();
        sut.AppendTo(sb);

        Assert.Equal("Start-a \n\rb \n\rc-End", sb.ToString());
    }

    [Fact]
    public void Should_ignore_full_whitespace_starts_and_ends()
    {
        var sut = new InnerTextOrHtml();

        sut.Add(" \n\r");
        sut.Add(" \n\r");
        sut.Add(" \n\rStart-");
        sut.Add("a");
        sut.Add("b");
        sut.Add("c");
        sut.Add("-End\n\r ");
        sut.Add(" \n\r");
        sut.Add(" \n\r");

        var sb = new StringBuilder();
        sut.AppendTo(sb);

        Assert.Equal("Start-abc-End", sb.ToString());
    }
}
