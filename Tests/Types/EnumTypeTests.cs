using Mjml.Net.Types;
using Xunit;

namespace Tests.Types
{
    public class EnumTypeTests
    {
        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("a")]
        [InlineData("b")]
        public void Should_validate_valid_values(string value)
        {
            var isValid = new EnumType("A", "B").Validate(value);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A ")]
        [InlineData("C")]
        [InlineData("c")]
        public void Should_validate_invalid_values(string value)
        {
            var isValid = new EnumType("A", "B").Validate(value);

            Assert.False(isValid);
        }
    }
}
