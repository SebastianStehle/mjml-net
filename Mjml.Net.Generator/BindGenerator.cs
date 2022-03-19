using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable IDE0056 // Use index operator

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

            var nonTextFields = allFields.Values.Where(x => !x.Text).ToList();

            var source = new SourceWriter();
            source.AppendLine("#pragma warning disable");
            source.AppendLine("// Auto-generated code");
            source.AppendLine("using Mjml.Net;");
            source.AppendLine();
            source.AppendLine($"namespace {namespaceName}");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"public partial class {classSymbol.Name}");
            source.AppendLine("{").MoveIn();
            source.AppendLine("public override void Bind(Mjml.Net.IBinder binder, Mjml.Net.GlobalContext context, System.Xml.XmlReader reader)");
            source.AppendLine("{").MoveIn();
            source.AppendLine("base.Bind(binder, context, reader);");

            foreach (var field in nonTextFields)
            {
                ProcessField(source, field, field == nonTextFields[nonTextFields.Count - 1]);
            }

            foreach (var field in nonTextFields)
            {
                ProcessFieldExpand(source, field, allFields);
            }

            var textField = allFields.Values.FirstOrDefault(x => x.Text);

            if (textField != null)
            {
                ProcessFieldText(source, textField);
            }

            source.MoveOut().AppendLine("}");
            source.AppendLine();
            source.AppendLine("public override AllowedAttributes AllowedFields");
            source.AppendLine("{").MoveIn();
            source.AppendLine("get");
            source.AppendLine("{").MoveIn();
            source.AppendLine("var result = new AllowedAttributes();");

            foreach (var field in nonTextFields)
            {
                ProcessFieldType(source, field);
            }

            source.AppendLine("return result;");
            source.MoveOut().AppendLine("}");
            source.MoveOut().AppendLine("}");
            source.AppendLine();
            source.AppendLine("public override string ? GetAttribute(string? name)");
            source.AppendLine("{").MoveIn();
            source.AppendLine("switch (name)");
            source.AppendLine("{").MoveIn();

            foreach (var field in nonTextFields)
            {
                ProcessFieldAttribute(source, field);
            }

            source.MoveOut().AppendLine("}");
            source.AppendLine("return null;");
            source.MoveOut().AppendLine("}");
            source.MoveOut().AppendLine("}");
            source.MoveOut().AppendLine("}");

            return source.ToString();
        }

        private void ProcessFieldText(SourceWriter source, FieldInfo field)
        {
            source.AppendLine($"{field.Name} = binder.GetText();");
        }

        private void ProcessFieldType(SourceWriter source, FieldInfo field)
        {
            source.AppendLine($"result[\"{field.Attribute}\"] = AttributeTypes.{field.Type};");
        }

        private void ProcessFieldAttribute(SourceWriter source, FieldInfo field)
        {
            source.AppendLine($"case \"{field.Attribute}\":").MoveIn();
            source.AppendLine($"return {field.Name};").MoveOut();
        }

        private void ProcessField(SourceWriter source, FieldInfo field, bool isLast)
        {
            var assignment = $"source{field.Name}";

            if (field.Type == "Color")
            {
                assignment = $"BindingHelper.CoerceColor(source{field.Name})";
            }

            source.AppendLine($"var source{field.Name} = binder.GetAttribute(\"{field.Attribute}\");");
            source.AppendLine($"if (source{field.Name} != null)");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"this.{field.Name} = {assignment};");
            source.MoveOut().AppendLine("}");

            if (!isLast)
            {
                source.AppendLine();
            }
        }

        private void ProcessFieldExpand(SourceWriter source, FieldInfo field, Dictionary<string, FieldInfo> allFields)
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

            source.AppendLine($"if ({fieldName} != null && ({fieldName}Top == null || {fieldName}Right == null || {fieldName}Bottom == null || {fieldName}Left == null))");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"var (t, r, b, l) = BindingHelper.ParseShorthandValue({fieldName});");
            source.AppendLine();
            source.AppendLine($"if ({fieldName}Top == null)");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"{fieldName}Top = t;");
            source.MoveOut().AppendLine("}");
            source.AppendLine($"if ({fieldName}Right == null)");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"{fieldName}Right = r;");
            source.MoveOut().AppendLine("}");
            source.AppendLine($"if ({fieldName}Bottom == null)");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"{fieldName}Bottom = b;");
            source.MoveOut().AppendLine("}");
            source.AppendLine($"if ({fieldName}Left == null)");
            source.AppendLine("{").MoveIn();
            source.AppendLine($"{fieldName}Left = l;");
            source.MoveOut().AppendLine("}");
            source.MoveOut().AppendLine("}");
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
