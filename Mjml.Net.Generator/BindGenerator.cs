using System.Text;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mjml.Net.Generator;

[Generator]
public class BindGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var templateStream = typeof(BindGenerator).Assembly.GetManifestResourceStream("Mjml.Net.Generator.Template.handlebar");
        var templateText = new StreamReader(templateStream).ReadToEnd();

        var template = Handlebars.Compile(templateText);

        static IEnumerable<FieldSource> Transform(GeneratorSyntaxContext ctx)
        {
            var classSyntax = (ClassDeclarationSyntax)ctx.Node;

            foreach (var field in classSyntax.Members.OfType<FieldDeclarationSyntax>())
            {
                foreach (var variable in field.Declaration.Variables)
                {
                    if (ctx.SemanticModel.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol)
                    {
                        yield break;
                    }

                    if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == Constants.BindAttributeName))
                    {
                        yield return new FieldSource(fieldSymbol, variable.Initializer?.Value.ToString()!, false);
                    }
                    else if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == Constants.BindTextAttributeName))
                    {
                        yield return new FieldSource(fieldSymbol, variable.Initializer?.Value.ToString()!, true);
                    }
                }
            }
        }

        var fieldDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) =>
            {
                return node is ClassDeclarationSyntax;
            },
            static (ctx, _) => Transform(ctx))
            .Where(x => x.Any());

        context.RegisterSourceOutput(fieldDeclarations, (context, fields) =>
        {
            var fieldsByClass = fields.GroupBy(f => f.Field.ContainingType, SymbolEqualityComparer.Default);

            foreach (var classFields in fieldsByClass)
            {
                var source = ProcessClass((INamedTypeSymbol)classFields.Key!, classFields.ToList(), template);

                context.AddSource($"{classFields.Key!.Name}_Binder.cs", SourceText.From(source, Encoding.UTF8));
            }
        });
    }

    private string ProcessClass(INamedTypeSymbol classSymbol, IEnumerable<FieldSource> fields, HandlebarsTemplate<object, object> template)
    {
        Console.WriteLine($"Handling {classSymbol.Name}");

        if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
        {
            return string.Empty;
        }

        var result = template(TemplateModel.Build(classSymbol, fields));

        return result;
    }
}
