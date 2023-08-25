namespace Mjml.Net;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public sealed record ValidationError(string Error, ValidationErrorType Type, int? LineNumber, int? LinePosition, string? File);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

public enum ValidationErrorType
{
    Other,
    InvalidAttribute,
    InvalidParent,
    UnknownAttribute,
    UnknownElement
}

public sealed class ValidationErrors : List<ValidationError>
{
    public void Add(string error, ValidationErrorType type, int? lineNumber = null, int? linePosition = null, string? file = null)
    {
        Add(new ValidationError(error, type, lineNumber, linePosition, file));
    }
}
