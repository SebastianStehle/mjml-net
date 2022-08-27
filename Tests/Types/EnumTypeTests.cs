using Mjml.Net;
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
            var context = default(ValidationContext);

            var isValid = new EnumType(false, "A", "B").Validate(value, ref context);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_allow_empty_string_when_optional(string value)
        {
            var context = default(ValidationContext);

            var isValid = new EnumType(true, "A", "B").Validate(value, ref context);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("A ")]
        [InlineData("C")]
        [InlineData("c")]
        public void Should_validate_invalid_values(string value)
        {
            var context = default(ValidationContext);

            var isValid = new EnumType(false, "A", "B").Validate(value, ref context);

            Assert.False(isValid);
        }
    }
}
