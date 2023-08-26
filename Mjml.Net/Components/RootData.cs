using System.Text;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Components;

public sealed record BodyBuffer(StringBuilder? Buffer) : GlobalData;

public sealed record Background(string Color) : GlobalData;

public sealed record HeadBuffer(StringBuilder? Buffer) : GlobalData;
