using Mjml.Net;
using Mjml.Net.Types;
using Xunit;

namespace Tests.Types
{
    public class NumberTypeTests
    {
        [Theory]
        [InlineData("0")]
        [InlineData("0 ")]
        [InlineData("10%")]
        [InlineData("10px")]
        [InlineData("10% ")]
        [InlineData("10px ")]
        [InlineData(" 10%")]
        [InlineData(" 10px")]
        public void Should_validate_valid_values(string value)
        {
            var context = default(ValidationContext);

            var isValid = new NumberType(Unit.Percent, Unit.Pixels).Validate(value, ref context);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2 px")]
        [InlineData("2 %")]
        [InlineData("0 rem")]
        [InlineData("0rem")]
        [InlineData("10 px ")]
        public void Should_validate_invalid_values(string value)
        {
            var context = default(ValidationContext);

            var isValid = new NumberType(Unit.Percent, Unit.Pixels).Validate(value, ref context);

            Assert.False(isValid);
        }
    }
}
