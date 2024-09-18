using Mjml.Net;

namespace Tests.Types;

public class UnitParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_parse_empty(string? value)
    {
        var result = UnitParser.Parse(value);

        Assert.Equal((0, Unit.Unknown), result);
    }

    [Fact]
    public void Should_parse_with_default_unit()
    {
        var result = UnitParser.Parse("42", Unit.Pixels);

        Assert.Equal((42, Unit.Pixels), result);
    }

    [Fact]
    public void Should_floor_pixels()
    {
        var result = UnitParser.Parse("60.5px", Unit.Pixels);

        Assert.Equal((60, Unit.Pixels), result);
    }

    [Fact]
    public void Should_floor_pixels_with_default_unit()
    {
        var result = UnitParser.Parse("60.5px", Unit.Pixels);

        Assert.Equal((60, Unit.Pixels), result);
    }

    [Fact]
    public void Should_parse_without_unit()
    {
        var result = UnitParser.Parse("60.5");

        Assert.Equal((60.5, Unit.None), result);
    }

    [Fact]
    public void Should_parse_without_unit2()
    {
        var result = UnitParser.Parse("60.5 ");

        Assert.Equal((60.5, Unit.None), result);
    }

    [Fact]
    public void Should_parse_without_value()
    {
        var result = UnitParser.Parse("px");

        Assert.Equal((0, Unit.Pixels), result);
    }

    [Fact]
    public void Should_parse_without_invalid()
    {
        var result = UnitParser.Parse("invalid");

        Assert.Equal((0, Unit.Unknown), result);
    }

    [Theory]
    [InlineData("100px")]
    [InlineData("100px ")]
    public void Should_parse_with_pixels(string value)
    {
        var result = UnitParser.Parse(value);

        Assert.Equal((100, Unit.Pixels), result);
    }

    [Theory]
    [InlineData("54.3%")]
    [InlineData("54.3% ")]
    public void Should_parse_with_percentage(string value)
    {
        var result = UnitParser.Parse(value);

        Assert.Equal((54.3, Unit.Percent), result);
    }

    [Theory]
    [InlineData("100px solid black")]
    [InlineData("100px solid black  ")]
    public void Should_parse_as_border(string value)
    {
        var result = UnitParser.Parse(value);

        Assert.Equal((100, Unit.Pixels), result);
    }
}
