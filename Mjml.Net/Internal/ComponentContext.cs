#pragma warning disable SA1401 // Fields should be private

namespace Mjml.Net.Internal
{
    internal sealed class ComponentContext
    {
        private bool isCopied;

        public readonly ChildOptions Options;

        public Dictionary<string, object?>? Values;

        public ComponentContext(ComponentContext? source, ChildOptions options)
        {
            if (source != null)
            {
                Values = source.Values;
            }

            Options = options;

            if (options.Values != null)
            {
                foreach (var (key, value) in options.Values)
                {
                    Set(key, value);
                }
            }
        }

        public void Set(string key, object? value)
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
