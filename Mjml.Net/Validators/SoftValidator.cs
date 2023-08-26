namespace Mjml.Net.Validators;

public sealed class SoftValidator : ValidatorBase
{
    public static readonly SoftValidator Instance = new SoftValidator();

    private SoftValidator()
        : base(false)
    {
    }
}
