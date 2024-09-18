using Tests.Internal;

namespace Tests.Components;

public class AccordionTests
{
    [Fact]
    public void Should_render_accordion()
    {
        var source = @"
<mj-accordion>
	<mj-accordion-element>
		<mj-accordion-title>Why use an accordion?</mj-accordion-title>
		<mj-accordion-text>
			<span style=""line-height:20px"">
				Element1
			</span>
		</mj-accordion-text>
	</mj-accordion-element>
	<mj-accordion-element>
		<mj-accordion-title>How it works</mj-accordion-title>
		<mj-accordion-text>
			<span style=""line-height:20px"">
				Element2
			</span>
		</mj-accordion-text>
	</mj-accordion-element>
</mj-accordion>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.Accordion.html", result);
    }

    [Fact]
    public void Should_render_accordion_with_empty_elements()
    {
        var source = @"
<mj-accordion>
    <mj-accordion-element>
    </mj-accordion-element>
    <mj-accordion-element>
    </mj-accordion-element>
</mj-accordion>";

        var (result, _) = TestHelper.Render(source);

        AssertHelpers.HtmlFileAssert("Components.Outputs.AccordionEmptyElements.html", result);
    }
}
