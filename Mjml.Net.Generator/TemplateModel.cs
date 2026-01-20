using Microsoft.CodeAnalysis;

namespace Mjml.Net.Generator;

internal sealed class TemplateModel
{
    public string ClassNamespace { get; set; }

    public string ClassName { get; set; }

    public Dictionary<string, TemplateField> Fields { get; init; }

    public IEnumerable<TemplateField> CustomTypes
    {
        get
        {
            return NormalFields.Where(x => x.IsCustom);
        }
    }

    public IEnumerable<TemplateField> DefaultTypes
    {
        get
        {
            return NormalFields.Where(x => !x.IsCustom);
        }
    }

    public IEnumerable<TemplateField> NormalFields
    {
        get
        {
            return Fields.Values.Where(x => !x.IsText).OrderBy(x => x.Name);
        }
    }

    public IEnumerable<TemplateField> TextFields
    {
        get
        {
            return Fields.Values.Where(x => x.IsText).OrderBy(x => x.Name);
        }
    }

    public IEnumerable<TemplateField> ExpandedFields
    {
        get
        {
            foreach (var normalField in NormalFields)
            {
                bool IsCandidate(string name)
                {
                    if (normalField.Attribute.Contains(name) &&
                       !normalField.Attribute.EndsWith($"{name}-top", StringComparison.Ordinal) &&
                       !normalField.Attribute.EndsWith($"{name}-right", StringComparison.Ordinal) &&
                       !normalField.Attribute.EndsWith($"{name}-bottom", StringComparison.Ordinal) &&
                       !normalField.Attribute.EndsWith($"{name}-left", StringComparison.Ordinal))
                    {
                        return true;
                    }

                    return false;
                }

                if (!IsCandidate("margin") && !IsCandidate("padding") && !IsCandidate("border"))
                {
                    continue;
                }

                if (Fields.ContainsKey($"{normalField.Name}Top") &&
                    Fields.ContainsKey($"{normalField.Name}Right") &&
                    Fields.ContainsKey($"{normalField.Name}Bottom") &&
                    Fields.ContainsKey($"{normalField.Name}Left"))
                {
                    yield return normalField;
                }
            }
        }
    }

    public static TemplateModel Build(INamedTypeSymbol classSymbol, IEnumerable<FieldSource> fields)
    {
        var allFields = new Dictionary<string, TemplateField>();

        var customTypes = 0;

        foreach (var source in fields)
        {
            var fieldInfo = new TemplateField
            {
                Name = source.Field.Name,
                Attribute = "none",
                DefaultValue = source.Value ?? "null",
                DefaultType = "String",
                IsText = source.AsText
            };

            var bindAttribute = source.Field.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == Constants.BindAttributeName);

            if (bindAttribute != null)
            {
                fieldInfo.Attribute = bindAttribute.ConstructorArguments.First().Value!.ToString()!;

                if (bindAttribute.ConstructorArguments.Length >= 2)
                {
                    var argument = bindAttribute.ConstructorArguments.Last();

                    if (argument.Type?.Name == "Type")
                    {
                        fieldInfo.CustomType = bindAttribute.ConstructorArguments!.Last().Value!.ToString()!;
                        fieldInfo.CustomName = $"__CustomType{customTypes}";

                        customTypes++;
                    }
                    else
                    {
                        // Value is an integer here so we need to convert it to its Enum.
                        var valueNumber = (int)bindAttribute.ConstructorArguments!.Last().Value!;
                        var valueString = argument.Type!.GetMembers()[valueNumber].Name;

                        fieldInfo.DefaultType = valueString;
                    }
                }
            }

            allFields[fieldInfo.Name] = fieldInfo;
        }

        return new TemplateModel
        {
            Fields = allFields,
            ClassName = classSymbol.Name,
            ClassNamespace = classSymbol.ContainingNamespace.ToDisplayString()
        };
    }
}
