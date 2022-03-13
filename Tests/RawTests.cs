using System.Collections.Generic;
using Mjml.Net;
using Xunit;

namespace Tests
{
    public class RawTests
    {
        public static IEnumerable<object[]> Tests()
        {
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
            var source = $@"
<mj-raw>{html}</mj-raw>";

            var result = TestHelper.Render(source);

            AssertHelpers.HtmlAssert(html, result);
        }
    }
}
