namespace Mjml.Net.Generator;

internal sealed record TemplateField
{
    public string Name { get; set; }

    public string Attribute { get; set; }

    public string DefaultValue { get; set; }

    public string DefaultType { get; set; }

    public string CustomType { get; set; }

    public string CustomName { get; set; }

    public bool IsText { get; set; }

    public bool IsBorder => Attribute is "border" or "inner-border";

    public bool IsCustom => CustomType != null;
}
