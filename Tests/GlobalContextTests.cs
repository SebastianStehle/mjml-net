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
        Assert.Null(sut.GetTypeAttributes("mj-text"));
        Assert.Null(sut.GetClassAttributes("class"));
        Assert.Null(sut.GetParentClassAttributes("parent-class", "mj-text"));
    }

    [Fact]
    public void Should_index_class_attributes()
    {
        var sut = new GlobalContext();

        sut.SetClassAttribute("color", "class", "red");
        sut.SetClassAttribute("color", "class", "blue");
        sut.SetClassAttribute("padding", "class", "10px");
        sut.SetParentClassAttribute("color", "parent-class", "mj-text", "green");

        Assert.Equal("blue", sut.GetClassAttributes("class")!["color"]);
        Assert.Equal("10px", sut.GetClassAttributes("class")!["padding"]);
        Assert.Equal("green", sut.GetParentClassAttributes("parent-class", "mj-text")!["color"]);
        Assert.Null(sut.GetClassAttributes("other"));
        Assert.Null(sut.GetParentClassAttributes("parent-class", "mj-button"));
    }
}
