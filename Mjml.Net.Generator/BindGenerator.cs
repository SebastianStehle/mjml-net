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

        private string ProcessClass(INamedTypeSymbol classSymbol, List<(IFieldSymbol Field, string Value, bool AsText)> fields, ISymbol attribute)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null;
            }

            var allFields = new Dictionary<string, FieldInfo>();

            foreach (var (field, value, text) in fields)
            {
                var fieldName = field.Name;
                var fieldType = "String";
                var fieldAttribute = "none";

                // Get the attribute name and the type.
                var bindAttribute = field.GetAttributes().FirstOrDefault(a => a.AttributeClass.Equals(attribute, SymbolEqualityComparer.Default));

                if (bindAttribute != null)
                {
                    fieldAttribute = bindAttribute.ConstructorArguments.First().Value.ToString();

                    if (bindAttribute.ConstructorArguments.Length >= 2)
                    {
                        var argument = bindAttribute.ConstructorArguments.Last();

                        // Value is an integer here so we need to convert it to its Enum.
                        var valueNumber = (int)bindAttribute.ConstructorArguments.Last().Value;
                        var valueString = argument.Type.GetMembers()[valueNumber].Name;

                        fieldType = valueString;
                    }
                }

                allFields[fieldName] = new FieldInfo(fieldName, fieldAttribute, value ?? "null", fieldType, text);
            }

            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            var source = new StringBuilder($@"#pragma warning disable
// Auto-generated code
namespace {namespaceName}
{{
    public partial class {classSymbol.Name}
    {{
        protected override void BindCore(INode node)
        {{
");
            foreach (var field in allFields.Values.Where(x => !x.Text))
            {
                ProcessField(source, field);
            }

            foreach (var field in allFields.Values.Where(x => !x.Text))
            {
                ProcessFieldExpand(source, field, allFields);
            }

            var textField = allFields.Values.FirstOrDefault(x => x.Text);

            if (textField != null)
            {
                ProcessFieldText(source, textField);
            }

            source.Append($@"
        }}

        public override AllowedAttributes AllowedFields
        {{
            get
            {{
                var result = new AllowedAttributes();
");
            foreach (var field in allFields.Values.Where(x => !x.Text))
            {
                ProcessFieldType(source, field);
            }

            source.Append($@"
                return result;
            }}
        }}

        public override string? GetDefaultValue(string? name)
        {{
            switch (name)
            {{
");
            foreach (var field in allFields.Values.Where(x => x.AttributeDefault != null && !x.Text))
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

        private void ProcessFieldText(StringBuilder source, FieldInfo field)
        {
            source.Append($@"
            {field.Name} = node.GetText();");
        }

        private void ProcessFieldType(StringBuilder source, FieldInfo field)
        {
            source.Append($@"
                result[""{field.Attribute}""] = AttributeTypes.{field.Type};");
        }

        private void ProcessFieldDefault(StringBuilder source, FieldInfo field)
        {
            source.Append($@"
                case ""{field.Attribute}"":
                    return {field.AttributeDefault};");
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
            }}");
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

        public bool Text { get; }

        public FieldInfo(string name, string attribute, string attributeDefault, string type, bool text)
        {
            Name = name;
            Attribute = attribute;
            AttributeDefault = attributeDefault;
            Type = type;
            Text = text;
        }
    }

    internal sealed class SyntaxReceiver : ISyntaxContextReceiver
    {
        public List<(IFieldSymbol Field, string Value, bool AsText)> Fields { get; } = new List<(IFieldSymbol Field, string Value, bool AsText)>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax && fieldDeclarationSyntax.AttributeLists.Count > 0)
            {
                foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
                {
                    var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;

                    if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "Mjml.Net.BindAttribute"))
                    {
                        Fields.Add((fieldSymbol, variable.Initializer?.Value.ToString(), false));
                    }
                    else if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass.ToDisplayString() == "Mjml.Net.BindTextAttribute"))
                    {
                        Fields.Add((fieldSymbol, variable.Initializer?.Value.ToString(), true));
                    }
                }
            }
        }
    }
}
