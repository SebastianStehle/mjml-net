namespace Mjml.Net
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    public sealed record ValidationError(string Error, int? Line, int? Column);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter

    public sealed class ValidationErrors : List<ValidationError>
    {
        public void Add(string error, int? line = null, int? column = null)
        {
            Add(new ValidationError(error, line, column));
        }
    }
}
