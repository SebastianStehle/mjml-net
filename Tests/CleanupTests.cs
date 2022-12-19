﻿using Mjml.Net;
using Xunit;

namespace Tests
{
    public class CleanupTests
    {
        private readonly IMjmlRenderer sut = new MjmlRenderer();

        [Fact]
        public void Should_not_cleanup_known_entity()
        {
            var source = "invalid tag &nbsp; found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid tag &nbsp; found", result);
        }

        [Fact]
        public void Should_cleanup_unknown_entity()
        {
            var source = "invalid tag &unknown; found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid tag &amp;unknown; found", result);
        }

        [Fact]
        public void Should_not_cleanup_uppercase_entity()
        {
            var source = "invalid tag &NBSP; found";

            var result = sut.FixXML(source);

            Assert.Equal("invalid tag &amp;NBSP; found", result);
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
        [InlineData("valid <br></br > found")]
        [InlineData("valid <br> </br> found")]
        [InlineData("valid <br> </br > found")]
        public void Should_not_cleanup_br_if_closed(string source)
        {
            var result = sut.FixXML(source);

            Assert.Equal(source, result);
        }
    }
}
