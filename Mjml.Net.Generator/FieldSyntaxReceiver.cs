using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
namespace Mjml.Net.Generator
{
    internal sealed class FieldSyntaxReceiver : ISyntaxContextReceiver
    {
        public List<(IFieldSymbol Field, string Value, bool AsText)> Fields { get; } = new List<(IFieldSymbol Field, string Value, bool AsText)>();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax && fieldDeclarationSyntax.AttributeLists.Count > 0)
            {
                foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
                {
                    if (context.SemanticModel.GetDeclaredSymbol(variable) is not IFieldSymbol fieldSymbol)
                    {
                        return;
                    }

                    if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "Mjml.Net.BindAttribute"))
                    {
                        Fields.Add((fieldSymbol, variable.Initializer?.Value.ToString()!, false));
                    }
                    else if (fieldSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "Mjml.Net.BindTextAttribute"))
                    {
                        Fields.Add((fieldSymbol, variable.Initializer?.Value.ToString()!, true));
                    }
                }
            }
        }
    }
}
