namespace Mjml.Net.Validators
{
    public sealed class SoftValidatorFactory : IValidatorFactory
    {
        public static readonly SoftValidatorFactory Instance = new SoftValidatorFactory();

        private SoftValidatorFactory()
        {
        }

        public IValidator Create()
        {
            return new SoftValidator();
        }
    }
}
