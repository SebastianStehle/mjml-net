using Mjml.Net;
using Xunit;

namespace Tests.Types
{
    public class ColorTypeTests
    {
        [Theory]
        [InlineData("rgba(100, 100, 100, 0.5)")]
        [InlineData("rgba(100, 100, 100, 0.5) ")]
        [InlineData("rgb(100, 100, 100)")]
        [InlineData("rgb(100, 100, 100) ")]
        [InlineData("#FF00FF")]
        [InlineData("#ff00ff")]
        [InlineData("#ff00ff ")]
        [InlineData("#f0f")]
        [InlineData("#f0f ")]
        [InlineData("red")]
        [InlineData("red ")]
        public void Should_validate_valid_values(string value)
        {
            var context = default(ValidationContext);

            var isValid = AttributeTypes.Color.Validate(value, ref context);

            Assert.True(isValid);
        }

        [Theory]
        [InlineData("#ff00bb", "#f0b")]
        [InlineData("#ff00bb", "#f0b ")]
        public void Should_coerce_value(string expected, string value)
        {
            var result = BindingHelper.CoerceColor(value);

            Assert.Equal(expected, result);
        }
    }
}
