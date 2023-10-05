using Mjml.Net.Components;
using Mjml.Net.Includes.Filesystem;
using Xunit;

namespace Tests;

public class FilesystemPathResolverTests
{
    [Fact]
    public void Should_CombinePathsWithNoBasePath()
    {
        /*
         * /
         *   rootTemplate.mjml
         *   level1/
         *     level1Template.mjml
         *     level2/
         *       level2Template.mjml <- start here
         *
         * includes - level2Template.mjml -> rootTemplate.mjml -> level1Template.mjml
         */

        var includeContext1 = new IncludedFileInfo("./..\\../level1\\..\\rootTemplate.mjml");
        var includeContext2 = new IncludedFileInfo("level1/level1Template.mjml", includeContext1);

        var resolver = new FilesystemPathResolver();
        var resolvedPath = resolver.ResolveFilePath(includeContext2);

        var expected = "./..\\../level1/level1Template.mjml";
        Assert.Equal(resolvedPath, expected, new FilePathEqualityComparer());
    }

    [Fact]
    public void Should_CombinePathsWithBasePath()
    {
        /*
         * C:/testFolder/
         *   rootTemplate.mjml
         *   level1/
         *     level1Template.mjml
         *     level2/
         *       level2Template.mjml <- start here
         *
         * includes - level2Template.mjml -> rootTemplate.mjml -> level1Template.mjml
         */

        var includeContext1 = new IncludedFileInfo("..\\../rootTemplate.mjml");
        var includeContext2 = new IncludedFileInfo("level1/level1Template.mjml", includeContext1);

        var resolver = new FilesystemPathResolver("C:/testFolder");
        var resolvedPath = resolver.ResolveFilePath(includeContext2);

        var expected = "C:/testFolder/..\\../level1/level1Template.mjml";
        Assert.Equal(resolvedPath, expected, new FilePathEqualityComparer());
    }
}
