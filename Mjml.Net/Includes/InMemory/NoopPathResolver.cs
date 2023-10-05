using Mjml.Net.Components;

namespace Mjml.Net.Includes.InMemory;

/// <summary>
///     A resolver that performs no action and just returns a value of <see cref="IncludedFileInfo.MjIncludeValue"/>.
/// </summary>
public class NoopPathResolver : IMjIncludePathResolver
{
    /// <inheritdoc />
    public string ResolveFilePath(IncludedFileInfo fileInfo) => fileInfo.MjIncludeValue;
}
