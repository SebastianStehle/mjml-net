#pragma warning disable SA1401 // Fields should be private

using System.Xml;

namespace Mjml.Net.Internal
{
    internal sealed class ComponentContext : IContext
    {
        private bool isCopied;

        public readonly ChildOptions Options;

        public Dictionary<string, object?>? Values;

        public XmlReader Reader { get; }

        public ComponentContext(ComponentContext? source, XmlReader reader, ChildOptions options)
        {
            Reader = reader;

            if (source != null)
            {
                Values = source.Values;
            }

            Options = options;
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
