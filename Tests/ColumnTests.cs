using Xunit;

namespace Tests
{
    public class ColumnTests
    {
        [Fact]
        public void Should_render_column_one()
        {
            var source = @"<mj-column background-color=""red""></mj-column>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("ColumnOne.html"), result);
        }

        [Fact]
        public void Should_render_column_two()
        {
            var source = @"
<mjml-test head=""false"">
    <mj-column background-color=""red""></mj-column>
    <mj-column background-color=""green""></mj-column>
</mjml-test>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("ColumnTwo.html"), result);
        }

        [Fact]
        public void Should_render_column_three()
        {
            var source = @"
<mjml-test head=""false"">
    <mj-column background-color=""red""></mj-column>
    <mj-column background-color=""green""></mj-column>
    <mj-column background-color=""blue""></mj-column>
</mjml-test>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("ColumnThree.html"), result);
        }

        [Fact]
        public void Should_render_column_four()
        {
            var source = @"
<mjml-test head=""false"">
    <mj-column background-color=""red""></mj-column>
    <mj-column background-color=""green""></mj-column>
    <mj-column background-color=""blue""></mj-column>
    <mj-column background-color=""yellow""></mj-column>
</mjml-test>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("ColumnFour.html"), result);
        }

        [Fact]
        public void Should_render_column_one_with_padding()
        {
            var source = @"<mj-column background-color=""red"" padding=""20px 52px""></mj-column>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(TestHelper.GetContent("ColumnOneWithPadding.html"), result);
        }
    }
}
