using Mjml.Net.Extensions;
using Xunit;

namespace Tests
{
    public class CssClassesExtensionsTests
    {
        [Fact]
        public void Should_suffix_single_class()
        {
            var cssClass = "class1";

            Assert.Equal("class1-outlook", cssClass.SuffixCssClasses("outlook"));
        }

        [Fact]
        public void Should_suffix_multiple_classes()
        {
            var cssClass = "class1 class2";

            Assert.Equal("class1-outlook class2-outlook", cssClass.SuffixCssClasses("outlook"));
        }

        [Fact]
        public void Should_suffix_no_classes()
        {
            var cssClass = "         ";

            Assert.Equal(string.Empty, cssClass.SuffixCssClasses("outlook"));
        }

        [Fact]
        public void Should_suffix_no_suffix()
        {
            var cssClass = "class1 class2";

            Assert.Equal("class1 class2", cssClass.SuffixCssClasses(string.Empty));
        }
    }
}
