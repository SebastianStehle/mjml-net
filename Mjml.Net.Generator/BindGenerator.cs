using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mjml.Net.Generator
{
    [Generator]
    public class BindGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var attribute = context.Compilation.GetTypeByMetadataName("Mjml.Net.BindAttribute");

            foreach (var group in receiver.Fields.GroupBy(f => f.Field.ContainingType, SymbolEqualityComparer.Default))
            {
                Console.WriteLine($"Handling {group.Key.Name}");

                var source = ProcessClass(group.Key as INamedTypeSymbol, group.ToList(), attribute);

                context.AddSource($"{group.Key.Name}_Binder.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        private string ProcessClass(INamedTypeSymbol classSymbol, List<(IFieldSymbol Field, string Value)> fields, ISymbol attribute)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null;
            }

            var allFields = new Dictionary<string, FieldInfo>();

            foreach (var (fiel, value) in fields)
            {
                var fieldName = fiel.Name;

                // Get the attribute name and the type.
                var attributeData = fiel.GetAttributes().Single(a => a.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default));
                var attributeName = attributeData.ConstructorArguments.First().Value;

                var type = "String";

                if (attributeData.ConstructorArguments.Length >= 2)
                {
                    var argument = attributeData.ConstructorArguments.Last();

                    // Value is an integer here so we need to convert it to its Enum.
                    var valueNumber = (int)attributeData.ConstructorArguments.Last().Value;
                    var valueString = argument.Type.GetMembers()[valueNumber].Name;

                    type = valueString;
                }

                allFields[fieldName] = new FieldInfo(fieldName, attributeName.ToString(), value ?? "null", type);
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var source = new StringBuilder($@"#pragma warning disable
// Auto-generated code
namespace {namespaceName}
{{
    public partial struct {classSymbol.Name} : IProps
    {{
        public {classSymbol.Name}(INode node)
            : this()
        {{
");
            foreach (var field in allFields.Values)
            {
                ProcessField(source, field);
            }

            foreach (var field in allFields.Values)
            {
                ProcessFieldExpand(source, field, allFields);
            }

            source.Append($@"
        }}

        public AllowedAttributes GetFields()
        {{
            var result = new AllowedAttributes();
");
            foreach (var field in allFields.Values)
            {
                ProcessFieldType(source, field);
            }

            source.Append($@"
            return result;
        }}

        public string? DefaultValue(string? name)
        {{
            switch (name)
            {{
");
            foreach (var field in allFields.Values.Where(x => x.AttributeDefault != null))
            {
                ProcessFieldDefault(source, field);
            }

            source.Append($@"
            }}
            return null;
        }}
    }}
}}");

            return source.ToString();
        }

        private void ProcessFieldType(StringBuilder source, FieldInfo field)
        {
            source.Append($@"
            result[""{field.Attribute}""] = AttributeTypes.{field.Type};
");
        }

        private void ProcessFieldDefault(StringBuilder source, FieldInfo field)
        {
            source.Append($@"
                case ""{field.Attribute}"":
                    return {field.AttributeDefault};
");
        }

        private void ProcessField(StringBuilder source, FieldInfo field)
        {
            var assignment = $"source{field.Name}";

            if (field.Type == "Color")
            {
                assignment = $"BindingHelper.CoerceColor(source{field.Name})";
            }

            source.Append($@"
            var source{field.Name} = node.GetAttribute(""{field.Attribute}"", true);
            if (source{field.Name} != null)
            {{
                this.{field.Name} = {assignment};
            }}
            else
            {{
                this.{field.Name} = {field.AttributeDefault};
            }}
");
        }

        private void ProcessFieldExpand(StringBuilder source, FieldInfo field, Dictionary<string, FieldInfo> allFields)
        {
            var fieldName = field.Name;

            var attribute = field.Attribute;

            bool IsCandidate(string name)
            {
                if (attribute.Contains(name) &&
                   !attribute.EndsWith($"{name}-top", StringComparison.Ordinal) &&
                   !attribute.EndsWith($"{name}-right", StringComparison.Ordinal) &&
                   !attribute.EndsWith($"{name}-bottom", StringComparison.Ordinal) &&
                   !attribute.EndsWith($"{name}-left", StringComparison.Ordinal))
                {
                    return true;
                }

                return false;
            }

            if (!IsCandidate("margin") && !IsCandidate("padding"))
            {
                return;
            }

            var otherFields = allFields.Values.Where(x =>
            {
                return
                    x.Name == $"{fieldName}Top" ||
                    x.Name == $"{fieldName}Right" ||
                    x.Name == $"{fieldName}Bottom" ||
                    x.Name == $"{fieldName}Left";
            }).ToList();

            if (otherFields.Count != 4)
            {
                return;
            }

            source.Append($@"
            if ({fieldName} != null && ({fieldName}Top == null || {fieldName}Right == null || {fieldName}Bottom == null || {fieldName}Left == null))
            {{
                var (t, r, b, l) = BindingHelper.ParseShorthandValue({fieldName});

                if ({fieldName}Top == null)
                {{
                    {fieldName}Top = t;
                }}

                if ({fieldName}Right == null)
                {{
                    {fieldName}Right = r;
                }}

                if ({fieldName}Bottom == null)
                {{
                    {fieldName}Bottom = b;
                }}

                if ({fieldName}Left == null)
                {{
                    {fieldName}Left = l;
                }}
            }}
");
        }
    }

    internal sealed class FieldInfo
    {
        public string Name { get; }

        public string Attribute { get; }

        public string AttributeDefault { get; }

        public string Type { get; }

        public FieldInfo(string name, string attribute, string attributeDefault, string type)
        {
            Name = name;
            Attribute = attribute;
            AttributeDefault = attributeDefault;
            Type = type;
        }
    }

    internal sealed class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<(IFieldSymbol Field, string Value)> Fields { get; } = new List<(IFieldSymbol Field, string Value)>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax && fieldDeclarationSyntax.AttributeLists.Count > 0)
            {
                foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
                {
                    var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;

                    if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "Mjml.Net.BindAttribute"))
                    {
                        Fields.Add((fieldSymbol, variable.Initializer?.Value.ToString()));
                    }
                }
            }
        }
    }
}
