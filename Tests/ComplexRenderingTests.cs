using Mjml.Net;
using System;
using System.Linq;
using Tests.Properties;
using Xunit;

namespace Tests
{
    public class ComplexRenderingTests
    {
        [Fact]
        public void Should_render_body_only()
        {
            var source = @"
 <mjml>
   <mj-body>
   </mj-body>
 </mjml>
";

            var result = new MjmlRenderer().Render(source, new MjmlOptions
            {
                Beautify = true,
            });

            // TrimmedEqual(Resources.BodyOnly, result);
        }
    }
}
