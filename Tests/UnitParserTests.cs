using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Assert.Equal((0, UnitType.Unknown), result);
        }

        [Fact]
        public void Should_parse_without_unit()
        {
            var result = UnitParser.Parse("60.5");

            Assert.Equal((60.5, UnitType.Unknown), result);
        }

        [Fact]
        public void Should_parse_without_value()
        {
            var result = UnitParser.Parse("px");

            Assert.Equal((0, UnitType.Pixels), result);
        }

        [Fact]
        public void Should_parse_without_invalid()
        {
            var result = UnitParser.Parse("invalid");

            Assert.Equal((0, UnitType.Unknown), result);
        }

        [Theory]
        [InlineData(" 100 px")]
        [InlineData("100 px")]
        [InlineData("100px")]
        [InlineData("100px ")]
        public void Should_parse_without_pixels(string value)
        {
            var result = UnitParser.Parse(value);

            Assert.Equal((100, UnitType.Pixels), result);
        }

        [Theory]
        [InlineData(" 54.3 %")]
        [InlineData("54.3 %")]
        [InlineData("54.3%")]
        [InlineData("54.3% ")]
        public void Should_parse_without_percentage(string value)
        {
            var result = UnitParser.Parse(value);

            Assert.Equal((54.3, UnitType.Percent), result);
        }
    }
}
