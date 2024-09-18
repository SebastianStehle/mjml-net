using Mjml.Net;
using Mjml.Net.Validators;

namespace Tests;

public class ValidationTests
{
    private readonly MjmlRenderer sut = new MjmlRenderer();

    [Fact]
    public void Should_add_error_if_root_not_mjml()
    {
        var source = @"
<mj-body>
</mj-body>
";

        var errors = Render(source);

        Assert.Equal(new[] { "'mj-body' cannot be the root tag." }, errors);
    }

    [Fact]
    public void Should_add_error_if_not_closed_properly()
    {
        var source = @"
 <mjml>
    <mj-body>
        <mj-section>
            <mj-column>
        </mj-section>
    </mj-body>
</mjml>
";

        var errors = Render(source);

        Assert.Equal(new[] { "Unexpected end element, expected 'mj-column', got 'mj-section'." }, errors);
    }

    [Fact]
    public void Should_add_error_if_not_closed_properly2()
    {
        var source = @"
 <mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-text>
                    <mj-column>
";

        var errors = Render(source);

        Assert.Equal(new[] { "Unexpected end element, expected 'mj-text', got 'Text' token." }, errors);
    }

    [Fact]
    public void Should_add_error_if_mj_body_not_found()
    {
        var source = @"
<mjml>
</mjml>
";

        var errors = Render(source);

        Assert.Equal(new[] { "Document must have 'mj-body' tag." }, errors);
    }

    [Fact]
    public void Should_add_error_if_tag_is_not_a_valid_child()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-body>
        </mj-body>
    </mj-body>
</mjml>
";

        var errors = Render(source);

        Assert.Equal(new[] { "'mj-body' must be child of 'mjml', found 'mj-body'." }, errors);
    }

    [Fact]
    public void Should_add_error_if_component_has_invalid_attribute()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-button invalid=""false"">Text</mj-button>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
";

        var errors = Render(source);

        Assert.Equal(new[] { "'invalid' is not a valid attribute of 'mj-button'." }, errors);
    }

    [Fact]
    public void Should_add_error_if_component_has_invalid_attribute_value()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-button width=""red"">Text</mj-button>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
";
        var errors = Render(source);

        Assert.Equal(new[] { "'red' is not a valid attribute 'width' of 'mj-button'." }, errors);
    }

    [Fact]
    public void Should_add_error_if_component_has_unexpected_text()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            hello
        </mj-section>
    </mj-body>
</mjml>
";
        var errors = Render(source);

        Assert.Equal(new[] { "Unexpected text content." }, errors);
    }

    [Fact]
    public void Should_add_error_if_component_has_unexpected_html_child()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <button></button>
        </mj-section>
    </mj-body>
</mjml>
";
        var errors = Render(source);

        Assert.Equal(new[] { "Invalid element 'button'." }, errors);
    }

    [Fact]
    public void Should_not_add_error_for_self_closing_element()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-divider />
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
";

        var errors = Render(source, SoftValidator.Instance);

        Assert.Empty(errors);
    }

    [Fact]
    public void Should_not_add_error_if_component_has_invalid_attribute_value_in_soft_mode()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-button width=""red"">Text</mj-button>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
";

        var errors = Render(source, SoftValidator.Instance);

        Assert.Empty(errors);
    }

    [Fact]
    public void Should_not_add_error_if_valid()
    {
        var source = @"
<mjml>
    <mj-body>
        <mj-section>
            <mj-column>
                <mj-button width=""10px"">Text</mj-button>
            </mj-column>
        </mj-section>
    </mj-body>
</mjml>
";

        var errors = Render(source);

        Assert.Empty(errors);
    }

    private string[] Render(string source, IValidator? validator = null)
    {
        var result = sut.Render(source, new MjmlOptions
        {
            Validator = validator ?? StrictValidator.Instance
        });

        return result.Errors.Select(x => x.Error).ToArray();
    }
}
