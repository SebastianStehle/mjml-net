namespace Mjml.Net
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BindAttribute : Attribute
    {
        public string Name { get; }

        public BindType Type { get; }

        public BindAttribute(string name, BindType type = BindType.String)
        {
            Name = name;

            Type = type;
        }
    }
}
