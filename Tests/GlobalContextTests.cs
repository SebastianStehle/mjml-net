using Mjml.Net;

namespace Tests;

public class GlobalContextTests
{
    [Fact]
    public void Should_clear_all_attributes()
    {
        var sut = new GlobalContext();

        sut.SetTypeAttribute("color", "mj-text", "red");
        sut.SetClassAttribute("color", "class", "red");
        sut.SetParentClassAttribute("color", "parent-class", "mj-text", "red");

        sut.Clear();

        Assert.Empty(sut.AttributesByName);
        Assert.Empty(sut.AttributesByClass);
        Assert.Empty(sut.AttributesByParentClass);
    }
}
