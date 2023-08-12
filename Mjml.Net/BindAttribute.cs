#pragma warning disable IDE0060 // Remove unused parameter

namespace Mjml.Net;

[AttributeUsage(AttributeTargets.Field)]
public sealed class BindAttribute : Attribute
{
    public BindAttribute(string name)
    {
    }

    public BindAttribute(string name, BindType type)
    {
    }

    public BindAttribute(string name, Type type)
    {
    }
}
