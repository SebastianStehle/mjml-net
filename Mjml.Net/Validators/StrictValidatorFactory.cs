namespace Mjml.Net.Validators;

public sealed class StrictValidatorFactory : IValidatorFactory
{
    public static readonly StrictValidatorFactory Instance = new StrictValidatorFactory();

    private StrictValidatorFactory()
    {
    }

    public IValidator Create()
    {
        return new StrictValidator();
    }
}
