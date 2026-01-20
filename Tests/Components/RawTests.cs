using Tests.Internal;

namespace Tests.Components;

public class RawTests
{
    public static readonly TheoryData<string> Tests = new TheoryData<string>
    {
            """
            Hello MJML
            """,
            """
            <strong>Hello</strong> MJML
            """,
            """
            <strong>Hello</strong> <strong>MJML</strong>
            """,
            """
            <strong>Hello</strong> MJML <strong>Whats Up</strong>
            """,
            """
            <strong>Hello</strong>Entity&nbsp;<strong>Whats Up</strong>
            """,
            """
            <button type="submit">Submit1</button>
            """,
            """
            <div>
                <button type="submit">Submit1</button>
            </div>
            """,
            """
            <div>
                <button type="submit">Submit1</button>
                <button type="submit">Submit2</button>
            </div>
            """,
            """
            <div>
                <table>
                    <button type="submit">Submit1</button>
                    <button type="submit">Submit2</button>
                </table>
            </div>
            """    };

    [Theory]
    [MemberData(nameof(Tests))]
    public void Should_render_raw_nested(string html)
    {
        var source = $@"<mj-raw>{html}</mj-raw>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlAssert(html, result);
    }

    [Fact]
    public void Should_render_raw_with_entity()
    {
        var source = $@"<mj-raw><div>&lt;</div></mj-raw>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlAssert("<div>&lt;</div>", result);
    }
}
