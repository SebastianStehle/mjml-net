using FakeItEasy;
using Mjml.Net;
using Mjml.Net.Components;
using Xunit;

namespace Tests
{
    public class NodeExtensionsTests
    {
        private readonly INode node = A.Fake<INode>();

        [Fact]
        public void Should_return_zero_if_no_border_found()
        {
            var result = node.GetShorthandBorderValue("border-left");

            Assert.Equal(0, result);
        }

        [Fact]
        public void Should_return_value_from_border()
        {
            A.CallTo(() => node.GetAttribute("border-left", null))
                .Returns("10px solid black");

            var result = node.GetShorthandBorderValue("border-left");

            Assert.Equal(10, result);
        }

        [Fact]
        public void Should_return_value_from_fallback_border()
        {
            A.CallTo(() => node.GetAttribute("border-left", null))
                .Returns(null);

            A.CallTo(() => node.GetAttribute("border", null))
                .Returns("10px solid black");

            var result = node.GetShorthandBorderValue("border-left");

            Assert.Equal(10, result);
        }

        [Fact]
        public void Should_return_zero_for_invalid_value()
        {
            A.CallTo(() => node.GetAttribute("border", null))
                .Returns("red solid black");

            var result = node.GetShorthandBorderValue("border-left");

            Assert.Equal(0, result);
        }
    }
}
