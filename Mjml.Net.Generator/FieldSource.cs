using Microsoft.CodeAnalysis;

#pragma warning disable RECS0082 // Parameter has the same name as a member and hides it

namespace Mjml.Net.Generator;

internal record FieldSource(IFieldSymbol Field, string Value, bool AsText);
