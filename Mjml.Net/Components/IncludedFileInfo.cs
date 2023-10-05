namespace Mjml.Net.Components;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// Represents information about included file.
/// </summary>
/// <param name="MjIncludeValue">The value that is indicated in path attribute of mj-include tag.</param>
/// <param name="ParentFileContext">The context of the file that included current file.</param>
public sealed record IncludedFileInfo(string MjIncludeValue, IncludedFileInfo? ParentFileContext = null)
{
    /// <summary>
    /// This field can be used to store any useful information related to included file.
    /// </summary>
    public dynamic Extras { get; set; }

    /// <summary>
    /// Returns whole chain of includes. Operates via <see cref="ParentFileContext"/>.
    /// </summary>
    /// <returns>A chain of previously included files.</returns>
    public IEnumerable<IncludedFileInfo> Flatten()
    {
        // Stack enumerates elements in reversed order.
        var result = new Stack<IncludedFileInfo>();
        var currentFileInfo = this;

        do
        {
            result.Push(currentFileInfo);
            currentFileInfo = currentFileInfo.ParentFileContext;
        }
        while (currentFileInfo != null);

        return result;
    }
}
