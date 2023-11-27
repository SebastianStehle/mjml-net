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
            return Fields.Values.Where(x => x.IsCustom).OrderByDescending(x => x.Name);
        }
    }

    public IEnumerable<TemplateField> DefaultTypes
    {
        get
        {
            return Fields.Values.Where(x => !x.IsCustom).OrderByDescending(x => x.Name);
        }
    }

    public IEnumerable<TemplateField> NormalFields
    {
        get
        {
            return Fields.Values.Where(x => !x.IsText).OrderByDescending(x => x.Name);
        }
    }

    public IEnumerable<TemplateField> TextFields
    {
        get
        {
            return Fields.Values.Where(x => x.IsText).OrderByDescending(x => x.Name);
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

        foreach (var field in allFields.Values)
        {
            bool IsCandidate(string name)
            {
                if (field.Attribute.Contains(name) &&
                   !field.Attribute.EndsWith($"{name}-top", StringComparison.Ordinal) &&
                   !field.Attribute.EndsWith($"{name}-right", StringComparison.Ordinal) &&
                   !field.Attribute.EndsWith($"{name}-bottom", StringComparison.Ordinal) &&
                   !field.Attribute.EndsWith($"{name}-left", StringComparison.Ordinal))
                {
                    return true;
                }

                return false;
            }

            if (!IsCandidate("margin") && !IsCandidate("padding") && !IsCandidate("border"))
            {
                continue;
            }

            if (allFields.ContainsKey($"{field.Name}Top") &&
                allFields.ContainsKey($"{field.Name}Right") &&
                allFields.ContainsKey($"{field.Name}Bottom") &&
                allFields.ContainsKey($"{field.Name}Left"))
            {
                field.IsExpanded = true;
            }
        }

        return new TemplateModel
        {
            Fields = allFields,
            ClassName = classSymbol.Name,
            ClassNamespace = classSymbol.ContainingNamespace.ToDisplayString()
        };
    }
}
