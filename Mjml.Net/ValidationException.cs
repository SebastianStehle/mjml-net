using System.Runtime.Serialization;

namespace Mjml.Net
{
    public sealed class ValidationException : Exception
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors)
            : base(BuildMessage(errors))
        {
            Errors = errors;
        }

        private static string BuildMessage(List<ValidationError> errors)
        {
            if (errors.Count == 1)
            {
                return errors[0].Error;
            }
            else
            {
                return string.Join(Environment.NewLine, errors.Select(x => x.Error));
            }
        }
    }
}
