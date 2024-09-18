using Tests.Internal;

namespace Tests.Components;

public class ColumnTests
{
    [Fact]
    public void Should_render_column_one()
    {
        var source = @"<mj-column background-color=""red""></mj-column>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnOne.html", result);
    }

    [Fact]
    public void Should_render_column_two()
    {
        var source = @"
<mjml-test head=""false"">
    <mj-column background-color=""red""></mj-column>
    <mj-column background-color=""green""></mj-column>
</mjml-test>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnTwo.html", result);
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

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnThree.html", result);
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

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnFour.html", result);
    }

    [Fact]
    public void Should_render_column_with_class()
    {
        var source = @"
<mjml-test head=""false"">
    <mj-attributes>
          <mj-table css-class=""test""></mj-table>
    </mj-attributes>
    <mj-column>
        <mj-table></mj-table>
    </mj-column>
</mjml-test>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnClass.html", result);
    }

    [Fact]
    public void Should_render_column_one_with_padding()
    {
        var source = @"<mj-column background-color=""red"" padding=""20px 52px""></mj-column>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.ColumnOneWithPadding.html", result);
    }
}
