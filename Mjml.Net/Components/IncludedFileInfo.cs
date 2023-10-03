namespace Mjml.Net.Components;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// Represents information about included file.
/// </summary>
/// <param name="MjIncludeValue">The value that is indicated in path attribute of mj-include tag.</param>
/// <param name="ParentFileContext">The context of the file that included current file.</param>
public sealed record IncludedFileInfo(string MjIncludeValue, IncludedFileInfo? ParentFileContext = null)
{
    private string? filePath;
    private string? directory;

    /// <summary>
    /// Returns a file name to read. This field is calculated based on all previously included files.
    /// </summary>
    public string FilePath => filePath ??= GetContextFilePath();

    /// <summary>
    /// Returns a directory for a file. This field is calculated based on all previously included files.
    /// </summary>
    public string Directory => directory ??= GetContextDirectory();

    /// <summary>
    /// This field can be used to store any useful information related to included file.
    /// </summary>
    public dynamic Extras { get; set; }

    private string GetContextDirectory()
    {
        // If the path of MjIncludeValue is absolute - it will be returned without joining.
        return Path.Combine(ParentFileContext?.Directory ?? string.Empty,
            Path.GetDirectoryName(MjIncludeValue) ?? string.Empty);
    }

    private string GetContextFilePath()
    {
        if (ParentFileContext is null)
        {
            return MjIncludeValue;
        }

        return Path.Combine(Directory, Path.GetFileName(MjIncludeValue));
    }
}
