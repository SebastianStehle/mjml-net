using Microsoft.CodeAnalysis;

namespace Mjml.Net.Generator;

internal sealed record FieldInfo
{
    public string Name { get; set; }

    public string Attribute { get; set; }

    public string DefaultValue { get; set; }

    public string DefaultType { get; set; }

    public string CustomType { get; set; }

    public string CustomTypeName { get; set; }

    public bool IsText { get; set; }

    public static Dictionary<string, FieldInfo> Build(List<(IFieldSymbol Field, string Value, bool AsText)> fields, ISymbol attribute)
    {
        var allFields = new Dictionary<string, FieldInfo>();

        var customTypes = 0;

        foreach (var (field, value, isText) in fields)
        {
            var fieldInfo = new FieldInfo
            {
                Name = field.Name,
                Attribute = "none",
                DefaultValue = value ?? "null",
                DefaultType = "String",
                IsText = isText
            };

            // Get the attribute name and the type.
            var bindAttribute = field.GetAttributes().FirstOrDefault(a => a.AttributeClass!.Equals(attribute, SymbolEqualityComparer.Default));

            if (bindAttribute != null)
            {
                fieldInfo.Attribute = bindAttribute.ConstructorArguments.First().Value!.ToString()!;

                if (bindAttribute.ConstructorArguments.Length >= 2)
                {
                    var argument = bindAttribute.ConstructorArguments.Last();

                    if (argument.Type?.Name == "Type")
                    {
                        fieldInfo.CustomType = bindAttribute.ConstructorArguments!.Last().Value!.ToString()!;
                        fieldInfo.CustomTypeName = $"__CustomType{customTypes}";

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

        return allFields;
    }
}
