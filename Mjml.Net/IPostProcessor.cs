namespace Mjml.Net;

public interface IPostProcessor
{
    string PostProcess(string html, MjmlOptions options);
}
