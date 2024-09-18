using Tests.Internal;

namespace Tests.Components;

public class TableTests
{
    [Fact]
    public void Should_render_table()
    {
        var source = @"
<mj-table>
    <tr style=""border-bottom:1px solid #ecedee;text-align:left;padding:15px 0;"">
        <th style=""padding: 0 15px 0 0;"">Year</th>
        <th style=""padding: 0 15px;"">Language</th>
        <th style=""padding: 0 0 0 15px;"">Inspired from</th>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">PHP</td>
        <td style=""padding: 0 0 0 15px;"">C, Shell Unix</td>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">JavaScript</td>
        <td style=""padding: 0 0 0 15px;"">Scheme, Self</td>
    </tr>
</mj-table>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Table.html", result);
    }

    [Fact]
    public void Should_render_table_pixels()
    {
        var source = @"
<mj-table width=""300px"">
    <tr style=""border-bottom:1px solid #ecedee;text-align:left;padding:15px 0;"">
        <th style=""padding: 0 15px 0 0;"">Year</th>
        <th style=""padding: 0 15px;"">Language</th>
        <th style=""padding: 0 0 0 15px;"">Inspired from</th>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">PHP</td>
        <td style=""padding: 0 0 0 15px;"">C, Shell Unix</td>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">JavaScript</td>
        <td style=""padding: 0 0 0 15px;"">Scheme, Self</td>
    </tr>
</mj-table>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TablePixels.html", result);
    }

    [Fact]
    public void Should_render_table_percent()
    {
        var source = @"
<mj-table width=""50%"">
    <tr style=""border-bottom:1px solid #ecedee;text-align:left;padding:15px 0;"">
        <th style=""padding: 0 15px 0 0;"">Year</th>
        <th style=""padding: 0 15px;"">Language</th>
        <th style=""padding: 0 0 0 15px;"">Inspired from</th>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">PHP</td>
        <td style=""padding: 0 0 0 15px;"">C, Shell Unix</td>
    </tr>
    <tr>
        <td style=""padding: 0 15px 0 0;"">1995</td>
        <td style=""padding: 0 15px;"">JavaScript</td>
        <td style=""padding: 0 0 0 15px;"">Scheme, Self</td>
    </tr>
</mj-table>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.TablePercent.html", result);
    }
}
