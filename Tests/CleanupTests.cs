﻿using Mjml.Net;
using Xunit;

namespace Tests
{
    public class CleanupTests
    {
        private readonly IMjmlRenderer sut = new MjmlRenderer();

        [Fact]
        public void Should_cleanup_and_in_attributes()
        {
            var source = " href=\"url&1=2\">";

            var result = sut.FixXML(source);

            Assert.Equal(" href=\"url&amp;1=2\">", result);
        }

        [Fact]
        public void Should_cleanup_entity()
        {
            var source = "invalid tag &nbsp; found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid tag &#160; found", result);
        }

        [Fact]
        public void Should_cleanup_uppercase_entity()
        {
            var source = "invalid tag &NBSP; found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid tag &#160; found", result);
        }

        [Fact]
        public void Should_cleanup_br_tags()
        {
            var source = "invalid <br> found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid <br /> found", result);
        }

        [Theory]
        [InlineData("valid <br></br> found")]
        [InlineData("valid <br></ br> found")]
        [InlineData("valid <br> </br> found")]
        [InlineData("valid <br> </ br> found")]
        public void Should_not_cleanup_br_if_closed(string source)
        {
            var result = sut.FixXML(source);

            Assert.Equal(source, result);
        }

        [Theory]
        [InlineData("<mj-image fluid-on-mobile=\"true\"></mj-image>")]
        [InlineData("<mj-image fluid-on-mobile=\"true\" background-color=\"#222222\"></mj-image>")]
        [InlineData("<mj-text background-color=\"#222222\"></mj-text>")]
        [InlineData("<mj-text background-color=\"#222222\" custom-tag-that-is-very-long=\"hello world\"></mj-text>")]
        public void Should_not_cleanup_and_not_break_multi_hyphen_attributes(string source)
        {
            var result = sut.FixXML(source);

            Assert.Equal(source, result);
        }
    }
}
