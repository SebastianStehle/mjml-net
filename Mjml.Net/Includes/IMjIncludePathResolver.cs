using Mjml.Net.Components;

namespace Mjml.Net.Includes;

public interface IMjIncludePathResolver
{
    string ResolveFilePath(IncludedFileInfo fileInfo);
}
