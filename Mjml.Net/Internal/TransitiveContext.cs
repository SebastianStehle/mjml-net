namespace Mjml.Net.Internal
{
    internal sealed class TransitiveContext : IContext
    {
        private bool isCopied;

        public Dictionary<string, object?>? Values;

        public TransitiveContext(TransitiveContext? source)
        {
            if (source != null)
            {
                Values = source.Values;
            }
        }

        public object? Set(string key, object? value)
        {
            if (Values == null)
            {
                Values = new Dictionary<string, object?>(1);
            }
            else if (!isCopied)
            {
                Values = new Dictionary<string, object?>(Values);

                isCopied = true;
            }

            Values[key] = value;

            return value;
        }

        public object? Get(string key)
        {
            if (Values == null)
            {
                return null;
            }

            return Values.GetValueOrDefault(key);
        }
    }
}
