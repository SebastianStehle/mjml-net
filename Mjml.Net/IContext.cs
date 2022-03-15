namespace Mjml.Net
{
    public interface IContext
    {
        object? Set(string key, object value);

        object? Get(string name);
    }
}
