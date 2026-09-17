using System.Reflection;
using AngleSharp;
using AngleSharp.Css.Parser;
using Mjml.Net;

namespace Tests;

public class AngleSharpPostProcessorTests
{
    private const string StyleAttributeObserverName = "StyleAttributeObserver";

    [Fact]
    public void Should_find_style_attribute_observer_in_css_configuration()
    {
        // The observer is removed by name, so a rename in AngleSharp.Css would silently bring back the expensive parsing.
        var configuration = Configuration.Default.WithCss(default(CssParserOptions));

        Assert.Contains(configuration.Services, x => x.GetType().Name == StyleAttributeObserverName);
    }

    [Fact]
    public void Should_remove_style_attribute_observer()
    {
        var field = typeof(AngleSharpPostProcessor).GetField("HtmlConfiguration", BindingFlags.NonPublic | BindingFlags.Static)!;
        var configuration = (IConfiguration)field.GetValue(null)!;

        Assert.DoesNotContain(configuration.Services, x => x.GetType().Name == StyleAttributeObserverName);
    }
}
