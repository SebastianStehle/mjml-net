# MJML.NET 

A blazingly-fast unofficial port of [MJML 4](https://mjml.io/) (by [MailJet](https://www.mailjet.com/)) to [.NET 6 & 7](https://dotnet.microsoft.com/).

[![Nuget](https://img.shields.io/nuget/v/Mjml.Net)](https://www.nuget.org/packages/Mjml.Net)

## Introduction

`MJML` is a markup language created by [Mailjet](https://www.mailjet.com/) and designed to reduce the pain of coding a responsive email. Its semantic syntax makes the language easy and straightforward while its rich standard components library shortens your development time and lightens your email codebase. MJML’s open-source engine takes care of translating the `MJML` you wrote into responsive HTML.

<p align="center">
  <a href="https://mjml.io" rel="nofollow">
    <img width="250" src="https://camo.githubusercontent.com/49c1d426dca03897940f39457ded0b622383efada70e7c845efadf68dfc8a73b/68747470733a2f2f6d6a6d6c2e696f2f6173736574732f696d672f6c69746d75732f6d6a6d6c62796d61696c6a65742e706e67" data-canonical-src="https://mjml.io/assets/img/litmus/mjmlbymailjet.png" style="max-width:100%;">
  </a>
</p>

<p align="center">
  <a href="https://mjml.io" target="_blank">
    <img width="75%" src="https://cloud.githubusercontent.com/assets/6558790/12450760/ee034178-bf85-11e5-9dda-98d0c8f9f8d6.png">
  </a>
</p>

<p align="center">
You can find out more about MJML 4 from the official website. 
</p>

<p align="center">
| <b><a href="https://mjml.io/">Official Website</a></b>
  | <b><a href="https://documentation.mjml.io/">Official Documentation</a></b>
  | <b><a href="https://mjml.io/getting-started-onboard">Official Onboarding</a></b>
  |
</p>

## Usage

Firstly, you'll need to reference the [`MJML.NET` NuGet Package](https://www.nuget.org/packages/Mjml.Net/) into your project.

```cmd
PM > Install-Package Mjml.Net
```

Secondly, include `MJML.NET` namespace into your project.
```csharp
using Mjml.Net;
```

Finally, the boilerplate code.
```csharp
public static void Main (string[] args) {
    var mjmlRenderer = new MjmlRenderer();

    string text = @"
        <mjml>
            <mj-head>
                <mj-title>Hello World Example</mj-title>
            </mj-head>
            <mj-body>
                <mj-section>
                    <mj-column>
                        <mj-text>
                            Hello World!
                        </mj-text>
                    </mj-column>
                </mj-section>
            </mj-body>
        </mjml>";

    var options = new MjmlOptions
    {
        Beautify = false
    };

    var (html, errors) = mjmlRenderer.Render(text, options);
}
```
### **Before Version 2.0.0**: XML Cleanup

MJML is not necessarily valid XML. To allow the XML parser to work properly, the MJML string needs to be sanitized before you render it. You can use the method ` FixXML` for that. If you store the MJML text in the database, it is store to keep the original version and the sanitized version. With version 2.0 we moved to html parser that follows the official standards and can therefore accept any kind of HTML input. More about this under "Known Issues".

```csharp
public static void Main (string[] args) {
    var mjmlRenderer = new MjmlRenderer();

    string text = @"
          <mjml>
              <mj-head>
                  <mj-title>Hello World Example</mj-title>
              </mj-head>
              <mj-body>
                  <mj-section>
                      <mj-column>
                          <mj-text>
                              Hello World!
                          </mj-text>
                      </mj-column>
                  </mj-section>
              </mj-body>
          </mjml>";

    var options = new MjmlOptions
    {
        Beautify = false
    };

    var xml = mjmlRenderer.FixXML(text, options);

    var (html, errors) = mjmlRenderer.Render(xml, options).Html;
}
```

## Options
You can also specify options to the MJML parser.

| Name             | Data Type                           | Default | Description                                                                                                                                                                                                         |
|------------------|-------------------------------------|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Beautify         | bool                                | true    | True to beatify the HTML. Impacts performance (slower).                                                                                                                                                             |
| Breakpoint       | string                              | 480px   | The default breakpoint to switch to mobile view.                                                                                                                                                                    |
| FileLoader       | IFileLoader?                        | null    | The file path loader for &lt;mj-include path="..." type="..."&gt; which handles loading the files from the specified path attribute. For example, SqlDatabaseFileLoader, InMemoryFileLoader, DiskFileLoader, etc... |
| Fonts            | IReadOnlyDictionary<string, Font[]> | Preset  | A list of supported default fonts.                                                                                                                                                                                  |
| ForceOWAQueries  | bool                                | false   | True to enable media queries for OWA.                                                                                                                                                                               |
| IdGenerator      | IIdGenerator                        | Preset  | The ID generator to create random values for attributes like Ids.                                                                                                                                                   |
| KeepComments     | bool                                | false   | True to keep comments.                                                                                                                                                                                              |
| Lax              | bool                                | false   | In lax mode some errors in the XML will be fixed. Only work when the MJML is passed in as string. Do not turn this on in production, because it can hurt performance.                                               |
| Minify           | bool                                | false   | True to minify the HTML. (**deprecated with version 2.0.0**)
| PostProcessors   | IPostProcessor[]?                   | []      | The post processors that will process the HTML, after rendering.                                                                                                                                                    |
| Styles           | Style[]?                            | []      | A list of custom styles.                                                                                                                                                                                            |
| ValidatorFactory | IValidatorFactory?                  | null    | The current validator, which also defines the validation mode.                                                                                                                                                      |
| XmlEntities      | IReadOnlyDictionary<string, string> | Preset  | A list of supported XML entities (**deprecated with version 1.16.0**)                                                                                                                                               |

## Supported Components

`MJML.NET` tries to implement all functionality `1-2-1` with the MJML 4 project. However, due to JavaScript not being a typed language this means there has been considerate refactoring to the code to make it more aligned with C# typed requirements. 

| Type | Component                                                               | Implemented        | Tests              | State            |
|------|-------------------------------------------------------------------------|--------------------|--------------------|------------------|
| Core | [mjml](https://documentation.mjml.io/#mjml)                             | :white_check_mark: | :white_check_mark: | Feature Complete |
| Core | [mj-head](https://documentation.mjml.io/#mj-head)                       | :white_check_mark: | :white_check_mark: | Feature Complete |
| Core | [mj-body](https://documentation.mjml.io/#mj-body)                       | :white_check_mark: | :white_check_mark: | Feature Complete |
| Core | [mj-include](https://documentation.mjml.io/#mj-include)                 | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-attributes](https://documentation.mjml.io/#mj-attributes)           | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | `mj-class`                                                              | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | `mj-all`                                                                | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-breakpoint](https://documentation.mjml.io/#mj-breakpoint)           | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-font](https://documentation.mjml.io/#mj-font)                       | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-html-attributes](https://documentation.mjml.io/#mj-html-attributes) | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-preview](https://documentation.mjml.io/#mj-preview)                 | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-style](https://documentation.mjml.io/#mj-style)                     | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-title](https://documentation.mjml.io/#mj-title)                     | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-accordion](https://documentation.mjml.io/#mj-accordion)             | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-button](https://documentation.mjml.io/#mj-button)                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-carousel](https://documentation.mjml.io/#mj-carousel)               | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-column](https://documentation.mjml.io/#mj-column)                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-divider](https://documentation.mjml.io/#mj-divider)                 | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-group](https://documentation.mjml.io/#mj-group)                     | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-hero](https://documentation.mjml.io/#mj-hero)                       | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-image](https://documentation.mjml.io/#mj-image)                     | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-navbar](https://documentation.mjml.io/#mj-navbar)                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-raw](https://documentation.mjml.io/#mj-raw)                         | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-section](https://documentation.mjml.io/#mj-section)                 | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-social](https://documentation.mjml.io/#mj-social)                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-spacer](https://documentation.mjml.io/#mj-spacer)                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-table](https://documentation.mjml.io/#mj-table)                     | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-text](https://documentation.mjml.io/#mj-text)                       | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-wrapper](https://documentation.mjml.io/#mj-wrapper)                 | :white_check_mark: | :white_check_mark: | Feature Complete |

## Inline Styles and Html Attributes

MJML supports inline styles (see: https://documentation.mjml.io/#mj-style) and html attributes (see: https://documentation.mjml.io/#mj-html-attributes). These are an expensive feature and can only be done after rendering. Therefore we have introduced a new post processing step, that applies inline styles and attributes using the AngleSharp library. To reduce the dependencies this has been moved to a separate nuget package.

```cmd
PM > Install-Package Mjml.Net.PostProcessors
```

You have to add the postprocessor to the options:

```csharp
var options = new MjmlOptions
{
    PostProcessors = [AngleSharpPostProcessor.Default]
};

var mjmlRenderer = new MjmlRenderer();
mjmlRenderer.AddHtmlAttributes();
var (html, errors) = await mjmlRenderer.RenderAsync(text, options);
```

Post processors can use async methods. Therefore we have introduced new methods to the mjml renderer. Only the asynchronous method variants will run post processors.

## Benchmarks

Performance was one of the key focuses for this project. We're aiming to support high
througput while mainintaing low memory footprint. Below are the benchmarks for every public MJML template compiled (beautified and minified). 

**Important: These tests do not include loading or saving the template from/to disk.**

If you'd like to run the benchmarks your self then you can run the `Mjml.Net.Benchmarks` project in `release` mode.

### BenchmarkDotNet
```
> .\Mjml.Net.Benchmark.exe
```

### Profiler
```
> .\Mjml.Net.Benchmark.exe -p -i 100
```

### Benchmark Specs
```ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19043.1586 (21H1/May2021Update)
Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-YRBKPS : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

Jit=RyuJit  Platform=X64  IterationCount=100 
```
### Benchmark Key
```
MjmlTemplateFilePath : Value of the 'MjmlTemplateFilePath' parameter
Mean                 : Arithmetic mean of all measurements
Error                : Half of 99.9% confidence interval
StdDev               : Standard deviation of all measurements
Median               : Value separating the higher half of all measurements (50th percentile)
Allocated            : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 us                 : 1 Microsecond (0.000001 sec)
```
### Benchmark Results

All times are in μs (1ms = 1000 μs)

| Method         | MjmlTemplateFilePath  | Mean        | Error     | StdDev    | Median      | Allocated    |
|----------------|-----------------------|-------------|-----------|-----------|-------------|--------------|
| **Beautified** | **Amario.mjml**       | **924.9**   | **7.24**  | **20.42** | **920.8**   | **810 KB**   |
| Minified       | Amario.mjml           | 980.1       | 25.57     | 75.39     | 974.1       | 810 KB       |
| **Beautified** | **Arturia.mjml**      | **438.2**   | **9.29**  | **27.38** | **438.9**   | **384 KB**   |
| Minified       | Arturia.mjml          | 430.0       | 8.62      | 25.28     | 430.9       | 384 KB       |
| **Beautified** | **Austin.mjml**       | **762.0**   | **13.06** | **38.30** | **759.7**   | **615 KB**   |
| Minified       | Austin.mjml           | 756.0       | 13.42     | 39.16     | 756.3       | 615 KB       |
| **Beautified** | **BlackFriday.mjml**  | **188.3**   | **4.39**  | **12.94** | **189.2**   | **186 KB**   |
| Minified       | BlackFriday.mjml      | 194.7       | 4.99      | 14.31     | 197.0       | 186 KB       |
| **Beautified** | **Card.mjml**         | **290.3**   | **7.10**  | **20.04** | **290.0**   | **259 KB**   |
| Minified       | Card.mjml             | 284.3       | 5.82      | 17.17     | 287.0       | 259 KB       |
| **Beautified** | **Christmas.mjml**    | **413.5**   | **8.55**  | **25.09** | **417.3**   | **360 KB**   |
| Minified       | Christmas.mjml        | 401.5       | 7.19      | 21.20     | 403.1       | 360 KB       |
| **Beautified** | **HappyNewYear.mjml** | **156.3**   | **3.30**  | **9.72**  | **156.4**   | **166 KB**   |
| Minified       | HappyNewYear.mjml     | 155.2       | 3.46      | 10.21     | 155.2       | 166 KB       |
| **Beautified** | **ManyHeroes.mjml**   | **705.7**   | **13.98** | **40.33** | **700.0**   | **830 KB**   |
| Minified       | ManyHeroes.mjml       | 667.6       | 9.15      | 26.41     | 653.7       | 830 KB       |
| **Beautified** | **OnePage.mjml**      | **414.9**   | **8.47**  | **24.58** | **403.5**   | **399 KB**   |
| Minified       | OnePage.mjml          | 424.5       | 9.15      | 26.83     | 416.4       | 399 KB       |
| **Beautified** | **Proof.mjml**        | **184.2**   | **2.90**  | **8.33**  | **181.6**   | **213 KB**   |
| Minified       | Proof.mjml            | 189.0       | 3.53      | 9.90      | 186.8       | 213 KB       |
| **Beautified** | **Racoon.mjml**       | **928.5**   | **8.38**  | **23.78** | **921.1**   | **810 KB**   |
| Minified       | Racoon.mjml           | 926.7       | 11.90     | 32.98     | 912.9       | 810 KB       |
| **Beautified** | **Reactivation.mjml** | **261.1**   | **4.12**  | **12.07** | **257.7**   | **259 KB**   |
| Minified       | Reactivation.mjml     | 263.5       | 5.63      | 16.34     | 260.6       | 259 KB       |
| **Beautified** | **RealEstate.mjml**   | **707.6**   | **11.65** | **33.43** | **701.2**   | **623 KB**   |
| Minified       | RealEstate.mjml       | 725.4       | 20.80     | 58.66     | 708.5       | 623 KB       |
| **Beautified** | **Recast.mjml**       | **674.6**   | **7.13**  | **20.12** | **666.0**   | **619 KB**   |
| Minified       | Recast.mjml           | 661.1       | 3.86      | 10.63     | 659.8       | 619 KB       |
| **Beautified** | **Receipt.mjml**      | **275.5**   | **2.64**  | **7.26**  | **272.8**   | **263 KB**   |
| Minified       | Receipt.mjml          | 274.5       | 1.93      | 5.50      | 272.8       | 263 KB       |
| **Beautified** | **Referral.mjml**     | **135.3**   | **0.75**  | **2.19**  | **134.9**   | **136 KB**   |
| Minified       | Referral.mjml         | 133.4       | 0.86      | 2.43      | 132.8       | 136 KB       |
| **Beautified** | **SpheroDroids.mjml** | **376.4**   | **2.28**  | **6.43**  | **374.7**   | **351 KB**   |
| Minified       | SpheroDroids.mjml     | 374.5       | 3.66      | 10.01     | 371.8       | 351 KB       |
| **Beautified** | **SpheroMini.mjml**   | **395.9**   | **4.43**  | **12.79** | **390.4**   | **368 KB**   |
| Minified       | SpheroMini.mjml       | 395.4       | 2.11      | 6.13      | 394.3       | 368 KB       |
| **Beautified** | **UGGRoyale.mjml**    | **1,126.0** | **6.03**  | **17.09** | **1,122.9** | **1,004 KB** |
| Minified       | UGGRoyale.mjml        | 1,154.0     | 10.93     | 31.89     | 1,142.2     | 1,004 KB     |
| **Beautified** | **Welcome.mjml**      | **125.1**   | **0.89**  | **2.49**  | **124.1**   | **131 KB**   |
| Minified       | Welcome.mjml          | 126.3       | 1.51      | 4.42      | 125.3       | 131 KB       |
| **Beautified** | **Worldly.mjml**      | **637.6**   | **3.21**  | **9.22**  | **634.9**   | **572 KB**   |
| Minified       | Worldly.mjml          | 629.6       | 4.24      | 11.60     | 627.9       | 572 KB       |

## Known Issues

### **Before Version 1.17.0**: Unknown HTML Entities

We are aware with an issue where by using HTML Character Entities (e.g. `&copy;`) are unknown and throw an unhandled exception during the rendering of the MJML document. This is because we use XmlReader as the main driver for parsing the MJML document.

The solution is to change the HTML Character Entity Names (e.g. `&copy;`) to there corresponding HTML Character Entity Number (e.g. `&#169;`) in the MJML document.
Here are some of the common HTML Character Entities:

| Result | Description                        | Entity Name | Entity Number |
|--------|------------------------------------|-------------|---------------|
|        | non-breaking space                 | `&nbsp;`    | `&#160;`      |
| <      | less than                          | `&lt`       | `&#60;`       |
| >      | greater than                       | `&gt;`      | `&#62;`       |
| &      | ampersand                          | `&amp;`     | `&#38;`       |
| "      | double quotation mark              | `&quot;`    | `&#34;`       |
| '      | single quotation mark (apostrophe) | `&apos;`    | `&#39;`       |
| ©      | copyright                          | `&copy;`    | `&#169;`      |
| ®      | registered trademark               | `&reg;`     | `&#174;`      |
| ™      | registered trademark               | `&trade;`   | `&#8482`      |

### **Before Version 2.0.0**: Non-encoded URL

We are aware of an issue with non-encoded URL's being recognized as character entities leading to an exception. This is because we use XmlReader as the main driver for parsing the MJML document. This solution is to URL encode all of the URLs in the template or turn on the lax mode with `options.Lax = true`.

### **Before Version 2.0.0**: Html tag that is not XML.

Some HTML tags are not valid HTML. Especially line break (`<br>`). Convert these tags to XHTML (`<br></br>` or `<br />`) or turn on the lax mode with `options.Lax = true`.

## Contribution

We really appreciate any contribution to the project to help provide a native version of MJML to C#. Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Appreciations

Once again, it's good to share some appreciation to the projects that make `MJML.NET` possible.

- [MJML](https://github.com/mjmlio/mjml)
- [MJML.NET V1](https://github.com/LiamRiddell/MJML.NET)
