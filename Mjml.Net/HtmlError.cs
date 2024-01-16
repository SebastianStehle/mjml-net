namespace Mjml.Net;

public record struct HtmlError(int LineNumber, int LinePosition, string Message);
