#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net;

public sealed record RenderResult(string Html, ValidationErrors Errors)
{
}
