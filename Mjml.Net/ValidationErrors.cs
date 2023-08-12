namespace Mjml.Net;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public sealed record ValidationError(string Error, ValidationErrorType Type, int? Line, int? Column);
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
    public void Add(string error, ValidationErrorType type, int? line = null, int? column = null)
    {
        Add(new ValidationError(error, type, line, column));
    }
}
