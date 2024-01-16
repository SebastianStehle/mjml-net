namespace Mjml.Net;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public sealed record ValidationError(string Error, ValidationErrorType Type, SourcePosition Position);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

public enum ValidationErrorType
{
    Other,
    InvalidAttribute,
    InvalidParent,
    UnknownAttribute,
    UnknownElement,
    InvalidHtml
}

public sealed class ValidationErrors : List<ValidationError>
{
    public ValidationErrors()
    {
    }

    public ValidationErrors(IEnumerable<ValidationError> source)
        : base(source)
    {
    }

    public void Add(string error, ValidationErrorType type, SourcePosition position = default)
    {
        Add(new ValidationError(error, type, position));
    }
}
