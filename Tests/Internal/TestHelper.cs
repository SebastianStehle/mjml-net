﻿using System.Globalization;
using Mjml.Net;

namespace Tests.Internal;

public static class TestHelper
{
    public static string Render(string source, MjmlOptions? options = null)
    {
        var renderer = new MjmlRenderer().AddList().Add<TestComponent>();

        options = BuildOptions(options);

        return renderer.Render(source, options).Html;
    }

    public static RenderResult RenderWithErrors(string source, MjmlOptions? options = null)
    {
        var renderer = new MjmlRenderer().AddList().Add<TestComponent>();

        options = BuildOptions(options);

        return renderer.Render(source, options);
    }

    public static string Render(string source, params IHelper[] helpers)
    {
        return Render(source, null, helpers);
    }

    public static async Task<string> RenderAsync(string source, MjmlOptions? options, params IHelper[] helpers)
    {
        var renderer = new MjmlRenderer().Add<TestComponent>().ClearHelpers();

        foreach (var helper in helpers)
        {
            renderer.Add(helper);
        }

        return (await renderer.RenderAsync(source, BuildOptions(options))).Html;
    }

    public static string Render(string source, MjmlOptions? options, params IHelper[] helpers)
    {
        var renderer = new MjmlRenderer().Add<TestComponent>().ClearHelpers();

        foreach (var helper in helpers)
        {
            renderer.Add(helper);
        }

        return renderer.Render(source, BuildOptions(options)).Html;
    }

    private static MjmlOptions BuildOptions(MjmlOptions? options)
    {
        options ??= new MjmlOptions();

        return options with
        {
            Beautify = true
        };
    }

    public static string GetContent(string content)
    {
        var stream = typeof(TestHelper).Assembly.GetManifestResourceStream($"Tests.{content}")!;

        return new StreamReader(stream).ReadToEnd();
    }

    public static void TestWithCulture(string cultureCode, Action action)
    {
        var culture = CultureInfo.GetCultureInfo(cultureCode);

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }
}
