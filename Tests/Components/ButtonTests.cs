﻿using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class ButtonTests
    {
        [Fact]
        public void Should_render_button()
        {
            var source = @"
<mj-button font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
    Button
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.Button.html", result);
        }

        [Fact]
        public void Should_render_button_link()
        {
            var source = @"
<mj-button href=""https://mjml.io/"" font-family=""Helvetica"" background-color=""#f45e43"" color=""white"">
    Button Link
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.ButtonLink.html", result);
        }

        [Fact]
        public void Should_render_button_link_with_rel()
        {
            var source = @"
<mj-button href=""https://mjml.io/"" font-family=""Helvetica"" background-color=""#f45e43"" color=""white"" rel=""relly good"">
    Button Link
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.ButtonLinkWithRel.html", result);
        }

        [Fact]
        public void Should_render_button_with_mixed_content()
        {
            var source = @"
<mj-button>
    <strong>Hello</strong> MJML 
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.ButtonMixedContent.html", result);
        }

        [Fact]
        public void Should_render_button_with_mixed_content2()
        {
            var source = @"
<mj-button>
    Hello <strong>MJML</strong>
</mj-button>
";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlFileAssert("Components.Outputs.ButtonMixedContent2.html", result);
        }
    }
}
