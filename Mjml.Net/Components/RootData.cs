using System.Text;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace Mjml.Net.Components;

public sealed record BodyBuffer(StringBuilder? Buffer) : GlobalData;

public sealed record Background(string Color) : GlobalData;

public sealed record HeadBuffer(StringBuilder? Buffer) : GlobalData;

public sealed record Language(string Value) : GlobalData;

public sealed record Direction(string Value) : GlobalData;

public sealed record ForceOWADesktop(bool Value) : GlobalData;
