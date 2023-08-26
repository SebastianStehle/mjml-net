namespace Mjml.Net;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

public record struct SourcePosition(
    int LineNumber,
    int LinePosition,
    string? File);
