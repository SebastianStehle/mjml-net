namespace Mjml.Net.Validators;

public sealed class StrictValidator : ValidatorBase
{
    public static readonly StrictValidator Instance = new StrictValidator();

    private StrictValidator()
        : base(true)
    {
    }
}
