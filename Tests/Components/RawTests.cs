using Tests.Internal;
using Xunit;

namespace Tests.Components
{
    public class RawTests
    {
        public static IEnumerable<object[]> Tests()
        {
            yield return new object[]
            {
                @"
Hello MJML"
            };

            yield return new object[]
            {
                @"
<strong>Hello</strong> MJML"
            };

            yield return new object[]
            {
                @"
<strong>Hello</strong> <strong>MJML</strong>"
            };

            yield return new object[]
            {
                @"
<strong>Hello</strong> MJML <strong>Whats Up</strong>"
            };

            yield return new object[]
            {
                @"
<button type=""submit"">Submit1</button>"
            };

            yield return new object[]
            {
                @"
<div>
    <button type=""submit"">Submit1</button>
</div>"
            };

            yield return new object[]
            {
                @"
<div>
    <button type=""submit"">Submit1</button>
    <button type=""submit"">Submit2</button>
</div>"
            };

            yield return new object[]
            {
                @"
<div>
    <table>
    <button type=""submit"">Submit1</button>
    <button type=""submit"">Submit2</button>
    </table>
</div>"
            };
        }

        [Theory]
        [MemberData(nameof(Tests))]
        public void Should_render_raw_nested(string html)
        {
            var source = $@"<mj-raw>{html}</mj-raw>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(html, result);
        }
    }
}
