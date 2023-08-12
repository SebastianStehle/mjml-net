using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable MA0098 // Use indexer instead of LINQ methods

namespace Mjml.Net.Generator;

[Generator]
public class BindGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new FieldSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not FieldSyntaxReceiver receiver)
        {
            return;
        }

        var attribute = context.Compilation.GetTypeByMetadataName("Mjml.Net.BindAttribute");

        foreach (var group in receiver.Fields.GroupBy(f => f.Field.ContainingType, SymbolEqualityComparer.Default))
        {
            var componentName = group.Key!.Name;

            Console.WriteLine($"Handling {componentName}");

            var source = ProcessClass((INamedTypeSymbol)group.Key, group.ToList(), attribute!);

            context.AddSource($"{componentName}_Binder.cs", SourceText.From(source, Encoding.UTF8));
        }
    }

    private string ProcessClass(INamedTypeSymbol classSymbol, List<(IFieldSymbol Field, string Value, bool AsText)> fields, ISymbol attribute)
    {
        if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            return string.Empty;
        }

        var fieldsInfo = FieldInfo.Build(fields, attribute);
        var fieldsNormal = fieldsInfo.Values.Where(x => !x.IsText).ToList();

        var source = new SourceWriter();
        source.AppendLine("#pragma warning disable");
        source.AppendLine("// Auto-generated code");
        source.AppendLine("using Mjml.Net;");
        source.AppendLine();
        source.AppendLine($"namespace {classSymbol.ContainingNamespace.ToDisplayString()};");
        source.AppendLine();
        source.AppendLine($"public partial class {classSymbol.Name}");
        source.AppendLine("{").MoveIn();

        var customTypes = fieldsInfo.Values.Where(x => x.CustomType != null);

        if (customTypes.Any())
        {
            foreach (var field in customTypes)
            {
                source.AppendLine($"public static readonly IType {field.CustomTypeName} = new {field.CustomType}();");
            }

            source.AppendLine();
        }

        GenerateBindMethod(fieldsInfo, fieldsNormal, source);

        source.AppendLine();

        GenerateAllowedFields(fieldsNormal, source);

        source.MoveOut().AppendLine("}");

        return source.ToString();
    }

    private void GenerateAllowedFields(List<FieldInfo> fieldsNormal, SourceWriter source)
    {
        source.AppendLine("public override AllowedAttributes AllowedFields");
        source.AppendLine("{").MoveIn();
        source.AppendLine("get");
        source.AppendLine("{").MoveIn();
        source.AppendLine("var result = new AllowedAttributes();");

        foreach (var field in fieldsNormal)
        {
            ProcessFieldType(source, field);
        }

        source.AppendLine();
        source.AppendLine("var inherited = base.AllowedFields;");
        source.AppendLine("if (inherited != null)");
        source.AppendLine("{").MoveIn();
        source.AppendLine("foreach (var (key, value) in inherited)");
        source.AppendLine("{").MoveIn();
        source.AppendLine("result[key] = value;");
        source.MoveOut().AppendLine("}");
        source.MoveOut().AppendLine("}");
        source.AppendLine("return result;");
        source.MoveOut().AppendLine("}");
        source.MoveOut().AppendLine("}");
        source.AppendLine();
        source.AppendLine("public override string ? GetAttribute(string? name)");
        source.AppendLine("{").MoveIn();
        source.AppendLine("switch (name)");
        source.AppendLine("{").MoveIn();

        foreach (var field in fieldsNormal)
        {
            ProcessFieldAttribute(source, field);
        }

        source.MoveOut().AppendLine("}");
        source.AppendLine("return base.GetAttribute(name);");
        source.MoveOut().AppendLine("}");
    }

    private void GenerateBindMethod(Dictionary<string, FieldInfo> fieldsInfo, List<FieldInfo> fieldsNormal, SourceWriter source)
    {
        source.AppendLine("public override void Bind(Mjml.Net.IBinder binder, Mjml.Net.GlobalContext context, Mjml.Net.IHtmlReader reader)");
        source.AppendLine("{").MoveIn();
        source.AppendLine("base.Bind(binder, context, reader);");

        foreach (var field in fieldsNormal)
        {
            ProcessFieldBinding(source, field, field == fieldsNormal.Last());
        }

        foreach (var field in fieldsNormal)
        {
            ProcessFieldExpand(source, field, fieldsInfo);
        }

        var textField = fieldsInfo.Values.FirstOrDefault(x => x.IsText);

        if (textField != null)
        {
            ProcessFieldText(source, textField);
        }

        source.MoveOut().AppendLine("}");
    }

    private void ProcessFieldText(SourceWriter source, FieldInfo field)
    {
        source.AppendLine($"{field.Name} = binder.GetText();");
    }

    private void ProcessFieldType(SourceWriter source, FieldInfo field)
    {
        if (field.CustomType != null)
        {
            source.AppendLine($"result[\"{field.Attribute}\"] = {field.CustomTypeName};");
        }
        else
        {
            source.AppendLine($"result[\"{field.Attribute}\"] = AttributeTypes.{field.DefaultType};");
        }
    }

    private void ProcessFieldAttribute(SourceWriter source, FieldInfo field)
    {
        source.AppendLine($"case \"{field.Attribute}\":").MoveIn();
        source.AppendLine($"return {field.Name};").MoveOut();
    }

    private void ProcessFieldBinding(SourceWriter source, FieldInfo field, bool isLast)
    {
        var assignment = $"source{field.Name}";

        if (field.DefaultType == "Color")
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

        if (!IsCandidate("margin") && !IsCandidate("padding") && !IsCandidate("border"))
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

        if (attribute == "border")
        {
            source.AppendLine($"var (t, r, b, l) = BindingHelper.ParseShorthandBorder({fieldName});");
        }
        else
        {
            source.AppendLine($"var (t, r, b, l) = BindingHelper.ParseShorthandValue({fieldName});");
        }

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
