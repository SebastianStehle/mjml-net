using Mjml.Net.Components;
using Xunit;

namespace Tests
{
    public class UnitParserTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Should_parse_empty(string value)
        {
            var result = UnitParser.Parse(value);

            Assert.Equal((0, Unit.Unknown), result);
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
        [InlineData(" 100 px")]
        [InlineData("100 px")]
        [InlineData("100px")]
        [InlineData("100px ")]
        public void Should_parse_without_pixels(string value)
        {
            var result = UnitParser.Parse(value);

            Assert.Equal((100, Unit.Pixels), result);
        }

        [Theory]
        [InlineData(" 54.3 %")]
        [InlineData("54.3 %")]
        [InlineData("54.3%")]
        [InlineData("54.3% ")]
        public void Should_parse_without_percentage(string value)
        {
            var result = UnitParser.Parse(value);

            Assert.Equal((54.3, Unit.Percent), result);
        }
    }
}
