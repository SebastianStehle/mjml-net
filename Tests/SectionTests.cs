﻿using Xunit;

namespace Tests
{
    public class SectionTests
    {
        [Fact]
        public void Should_render_section()
        {
            var source = @"<mj-section></mj-section>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("Section.html"), result);
        }

        [Fact]
        public void Should_render_section_with_background_color()
        {
            var source = @"<mj-section background-color=""red""></mj-section>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("SectionWithBackgroundColor.html"), result);
        }
    }
}
