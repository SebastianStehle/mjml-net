using Mjml.Net;
using Tests.Internal;
using Xunit;

namespace Tests.Components;

public class CommentTests
{
    [Fact]
    public void Should_keep_comments()
    {
        var source = @"
<mjml-test head=""false"">
    <!-- COMMENT 1 -->
    <mj-spacer />
    <!-- COMMENT 2 -->
    <mj-spacer />
    <!-- COMMENT 3 -->
</mjml-test>
";

        var result = TestHelper.Render(source, new MjmlOptions
        {
            KeepComments = true
        });

        AssertHelpers.HtmlFileAssert("Components.Outputs.Comments.html", result);
    }
}
