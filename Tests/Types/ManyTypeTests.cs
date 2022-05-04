using Mjml.Net;
using Mjml.Net.Types;
using Xunit;

namespace Tests.Types
{
    public class ManyTypeTests
    {
        [Theory]
        [InlineData("0")]
        [InlineData("0 ")]
        [InlineData("10%")]
        [InlineData("10px")]
        [InlineData("10px 20px")]
        [InlineData("10px 20px 30px")]
        [InlineData("10px 20px 30px 40px")]
        [InlineData("10px 20px 30px  40px")]
        public void Should_validate_valid_values(string value)
        {
            var isValid = new ManyType(new NumberType(Unit.Percent, Unit.Pixels), 1, 4).Validate(value);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2 px")]
        [InlineData("2 %")]
        [InlineData("0 rem")]
        [InlineData("0rem")]
        [InlineData("10 px ")]
        [InlineData("1px 2px 3px 4px 5px")]
        public void Should_validate_invalid_values(string value)
        {
            var isValid = new ManyType(new NumberType(Unit.Percent, Unit.Pixels), 1, 4).Validate(value);

            Assert.False(isValid);
        }
    }
}
