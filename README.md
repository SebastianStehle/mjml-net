# MJML.NET 

A blazingly-fast unofficial port of [MJML](https://mjml.io/) (by [MailJet](https://www.mailjet.com/)) to [.NET 6](https://dotnet.microsoft.com/).

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

Firstly, you'll need to reference the `MJML.NET 2` NuGet Package into your project.

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
    var mjmlParser = new MjmlRenderer();

    string mjml = @"
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

    var html = mjmlRenderer.Render(text, new MjmlOptions {
        Beautify = false
    }).Html;
}
```
## Options
You can also specify options to the MJML parser.

|       Name       |              Data Type              |           Default           |                                                                              Description                                                                              |
|:----------------:|:-----------------------------------:|:---------------------------:|:---------------------------------------------------------------------------------------------------------------------------------------------------------------------:|
|   KeepComments   |                 bool                |            false            |                                                                         True to keep comments.                                                                        |
|    Breakpoint    |                string               |            480px            |                                                            The default breakpoint to switch to mobile view.                                                           |
|      Styles      |               Style[]               |              []             |                                                                        A list of custom styles.                                                                       |
|  ForceOWAQueries |                 bool                |            false            |                                                                 True to enable media queries for OWA.                                                                 |
|     Beautify     |                 bool                |             true            |                                                                       True to beatify the HTML. Impacts performance (slower).                                                                 |
|      Minify      |                 bool                |            false            |                                                                        True to minify the HTML.                                                                       |
|        Lax       |                 bool                |            false            | In lax mode some errors in the XML will be fixed. Only work when the MJML is passed in as string. Do not turn this on in production, because it can hurt performance. |
|    IdGenerator   | IIdGenerator                        | DefaultIDGenerator.Instance | The ID generator to create random values for attributes like Ids.                                                                                                     |
| Fonts            | IReadOnlyDictionary<string, Font>   | DefaultFonts                | A list of supported default fonts.                                                                                                                                    |
|    XmlEntities   | IReadOnlyDictionary<string, string> |      DefaultXmlEntities     |                                                                   A list of supported XML entities.                                                                   |
| ValidatorFactory | IValidatorFactory?                  | null                        | The current validator.                                                                                                                                                |

## Supported Components
`MJML.NET` tries to implement all functionality `1-2-1` with the MJML 4 project. However, due to JavaScript not being a typed language this means there has been considerate refactoring to the code to make it more aligned with C# typed requirements. 

| Type | Component                                                                  | Implemented        | Tests              | State            |
| ---- | -------------------------------------------------------------------------- | ------------------ | ------------------ | ---------------- |
| Core | [mjml](https://documentation.mjml.io/#mjml)                                | :white_check_mark: | :white_check_mark: | Feature Complete |
| Core | [mj-head](https://documentation.mjml.io/#mj-head)                          | :white_check_mark: | :white_check_mark: | Feature Complete | 
| Core | [mj-body](https://documentation.mjml.io/#mj-body)                          | :white_check_mark: | :white_check_mark: | Feature Complete |
| Core | [mj-include](https://documentation.mjml.io/#mj-include)                    | :x:                | :x:                | Not Planned      |
| Head | [mj-attributes](https://documentation.mjml.io/#mj-attributes)              | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | `mj-class`                                                                 | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | `mj-all`                                                                   | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-breakpoint](https://documentation.mjml.io/#mj-breakpoint)              | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-font](https://documentation.mjml.io/#mj-font)                          | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-html-attributes](https://documentation.mjml.io/#mj-html-attributes)    | :x:                | :x:                | Not Planned      |
| Head | [mj-preview](https://documentation.mjml.io/#mj-preview)                    | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-style](https://documentation.mjml.io/#mj-style)                        | :white_check_mark: | :white_check_mark: | Feature Complete |
| Head | [mj-title](https://documentation.mjml.io/#mj-title)                        | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-accordion](https://documentation.mjml.io/#mj-accordion)                | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-button](https://documentation.mjml.io/#mj-button)                      | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-carousel](https://documentation.mjml.io/#mj-carousel)                  | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-column](https://documentation.mjml.io/#mj-column)                      | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-divider](https://documentation.mjml.io/#mj-divider)                    | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-group](https://documentation.mjml.io/#mj-group)                        | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-hero](https://documentation.mjml.io/#mj-hero)                          | :white_check_mark: | :white_check_mark: | Feature Complete | 
| Body | [mj-image](https://documentation.mjml.io/#mj-image)                        | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-navbar](https://documentation.mjml.io/#mj-navbar)                      | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-raw](https://documentation.mjml.io/#mj-raw)                            | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-section](https://documentation.mjml.io/#mj-section)                    | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-social](https://documentation.mjml.io/#mj-social)                      | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-spacer](https://documentation.mjml.io/#mj-spacer)                      | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-table](https://documentation.mjml.io/#mj-table)                        | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-text](https://documentation.mjml.io/#mj-text)                          | :white_check_mark: | :white_check_mark: | Feature Complete |
| Body | [mj-wrapper](https://documentation.mjml.io/#mj-wrapper)                    | :white_check_mark: | :white_check_mark: | Feature Complete |

## Benchmarks
Performance was one of the key focuses for this project. We're aiming to support high
througput while mainintaing low memory footprint. Below are the benchmarks for every public MJML template compiled (beautified and minified). 

**Important: These tests do not include loading or saving the template from/to disk.**

If you'd like to run the benchmarks your self then you can run the `Mjml.Net.Benchmarks` project in `release` mode.

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
Gen 0                : GC Generation 0 collects per 1000 operations
Gen 1                : GC Generation 1 collects per 1000 operations
Gen 2                : GC Generation 2 collects per 1000 operations
Allocated            : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 us                 : 1 Microsecond (0.000001 sec)
```
### Benchmark Results

|                   Method | MjmlTemplateFilePath |       Mean |    Error |   StdDev |     Median |    Gen 0 |    Gen 1 |    Gen 2 | Allocated |
|------------------------- |--------------------- |-----------:|---------:|---------:|-----------:|---------:|---------:|---------:|----------:|
| **Render_Template_Beautify** | **Amario.mjml** |   **924.9 μs** |  **7.24 μs** | **20.42 μs** |   **920.8 μs** | **109.3750** | **103.5156** | **103.5156** |    **810 KB** |
|   Render_Template_Minify | Amario.mjml |   980.1 μs | 25.57 μs | 75.39 μs |   974.1 μs | 109.3750 | 103.5156 | 103.5156 |    810 KB |
| **Render_Template_Beautify** | **Arturia.mjml** |   **438.2 μs** |  **9.29 μs** | **27.38 μs** |   **438.9 μs** |  **83.0078** |  **52.7344** |  **27.3438** |    **384 KB** |
|   Render_Template_Minify | Arturia.mjml |   430.0 μs |  8.62 μs | 25.28 μs |   430.9 μs |  83.0078 |  27.3438 |  27.3438 |    384 KB |
| **Render_Template_Beautify** | **Austin.mjml** |   **762.0 μs** | **13.06 μs** | **38.30 μs** |   **759.7 μs** |  **94.7266** |  **94.7266** |  **94.7266** |    **615 KB** |
|   Render_Template_Minify | Austin.mjml |   756.0 μs | 13.42 μs | 39.16 μs |   756.3 μs |  94.7266 |  94.7266 |  94.7266 |    615 KB |
| **Render_Template_Beautify** | **BlackFriday.mjml** |   **188.3 μs** |  **4.39 μs** | **12.94 μs** |   **189.2 μs** |  **45.4102** |   **6.3477** |        **-** |    **186 KB** |
|   Render_Template_Minify | BlackFriday.mjml |   194.7 μs |  4.99 μs | 14.31 μs |   197.0 μs |  45.4102 |   6.3477 |        - |    186 KB |
| **Render_Template_Beautify** | **Card.mjml** |   **290.3 μs** |  **7.10 μs** | **20.04 μs** |   **290.0 μs** |  **62.0117** |  **20.9961** |        **-** |    **259 KB** |
|   Render_Template_Minify | Card.mjml |   284.3 μs |  5.82 μs | 17.17 μs |   287.0 μs |  62.0117 |  20.9961 |        - |    259 KB |
| **Render_Template_Beautify** | **Christmas.mjml** |   **413.5 μs** |  **8.55 μs** | **25.09 μs** |   **417.3 μs** |  **74.7070** |  **24.9023** |        **-** |    **360 KB** |
|   Render_Template_Minify | Christmas.mjml |   401.5 μs |  7.19 μs | 21.20 μs |   403.1 μs |  74.7070 |  24.9023 |        - |    360 KB |
| **Render_Template_Beautify** | **HappyNewYear.mjml** |   **156.3 μs** |  **3.30 μs** |  **9.72 μs** |   **156.4 μs** |  **40.5273** |  **10.0098** |        **-** |    **166 KB** |
|   Render_Template_Minify | HappyNewYear.mjml |   155.2 μs |  3.46 μs | 10.21 μs |   155.2 μs |  40.5273 |  10.0098 |        - |    166 KB |
| **Render_Template_Beautify** | **ManyHeroes.mjml** |   **705.7 μs** | **13.98 μs** | **40.33 μs** |   **700.0 μs** | **142.5781** | **142.5781** | **142.5781** |    **830 KB** |
|   Render_Template_Minify | ManyHeroes.mjml |   667.6 μs |  9.15 μs | 26.41 μs |   653.7 μs | 142.5781 | 142.5781 | 142.5781 |    830 KB |
| **Render_Template_Beautify** | **OnePage.mjml** |   **414.9 μs** |  **8.47 μs** | **24.58 μs** |   **403.5 μs** |  **80.0781** |  **38.0859** |        **-** |    **399 KB** |
|   Render_Template_Minify | OnePage.mjml |   424.5 μs |  9.15 μs | 26.83 μs |   416.4 μs |  80.0781 |  39.0625 |        - |    399 KB |
| **Render_Template_Beautify** | **Proof.mjml** |   **184.2 μs** |  **2.90 μs** |  **8.33 μs** |   **181.6 μs** |  **51.7578** |   **0.4883** |        **-** |    **213 KB** |
|   Render_Template_Minify | Proof.mjml |   189.0 μs |  3.53 μs |  9.90 μs |   186.8 μs |  51.7578 |   0.4883 |        - |    213 KB |
| **Render_Template_Beautify** | **Racoon.mjml** |   **928.5 μs** |  **8.38 μs** | **23.78 μs** |   **921.1 μs** | **109.3750** | **103.5156** | **103.5156** |    **810 KB** |
|   Render_Template_Minify | Racoon.mjml |   926.7 μs | 11.90 μs | 32.98 μs |   912.9 μs | 109.3750 | 103.5156 | 103.5156 |    810 KB |
| **Render_Template_Beautify** | **Reactivation.mjml** |   **261.1 μs** |  **4.12 μs** | **12.07 μs** |   **257.7 μs** |  **60.0586** |  **13.6719** |        **-** |    **259 KB** |
|   Render_Template_Minify | Reactivation.mjml |   263.5 μs |  5.63 μs | 16.34 μs |   260.6 μs |  60.0586 |  13.6719 |        - |    259 KB |
| **Render_Template_Beautify** | **RealEstate.mjml** |   **707.6 μs** | **11.65 μs** | **33.43 μs** |   **701.2 μs** |  **94.7266** |  **94.7266** |  **94.7266** |    **623 KB** |
|   Render_Template_Minify | RealEstate.mjml |   725.4 μs | 20.80 μs | 58.66 μs |   708.5 μs |  94.7266 |  94.7266 |  94.7266 |    623 KB |
| **Render_Template_Beautify** | **Recast.mjml** |   **674.6 μs** |  **7.13 μs** | **20.12 μs** |   **666.0 μs** | **152.3438** |  **76.1719** |  **76.1719** |    **619 KB** |
|   Render_Template_Minify | Recast.mjml |   661.1 μs |  3.86 μs | 10.63 μs |   659.8 μs | 152.3438 |  76.1719 |  76.1719 |    619 KB |
| **Render_Template_Beautify** | **Receipt.mjml** |   **275.5 μs** |  **2.64 μs** |  **7.26 μs** |   **272.8 μs** |  **63.4766** |  **20.9961** |        **-** |    **263 KB** |
|   Render_Template_Minify | Receipt.mjml |   274.5 μs |  1.93 μs |  5.50 μs |   272.8 μs |  63.4766 |  20.9961 |        - |    263 KB |
| **Render_Template_Beautify** | **Referral.mjml** |   **135.3 μs** |  **0.75 μs** |  **2.19 μs** |   **134.9 μs** |  **32.9590** |        **-** |        **-** |    **136 KB** |
|   Render_Template_Minify | Referral.mjml |   133.4 μs |  0.86 μs |  2.43 μs |   132.8 μs |  32.9590 |        - |        - |    136 KB |
| **Render_Template_Beautify** | **SpheroDroids.mjml** |   **376.4 μs** |  **2.28 μs** |  **6.43 μs** |   **374.7 μs** |  **80.0781** |  **52.7344** |  **26.8555** |    **351 KB** |
|   Render_Template_Minify | SpheroDroids.mjml |   374.5 μs |  3.66 μs | 10.01 μs |   371.8 μs |  71.2891 |  26.8555 |  26.8555 |    351 KB |
| **Render_Template_Beautify** | **SpheroMini.mjml** |   **395.9 μs** |  **4.43 μs** | **12.79 μs** |   **390.4 μs** |  **70.3125** |  **26.8555** |  **26.8555** |    **368 KB** |
|   Render_Template_Minify | SpheroMini.mjml |   395.4 μs |  2.11 μs |  6.13 μs |   394.3 μs |  71.7773 |  26.8555 |  26.8555 |    368 KB |
| **Render_Template_Beautify** | **UGGRoyale.mjml** | **1,126.0 μs** |  **6.03 μs** | **17.09 μs** | **1,122.9 μs** | **142.5781** | **142.5781** | **142.5781** |  **1,004 KB** |
|   Render_Template_Minify | UGGRoyale.mjml | 1,154.0 μs | 10.93 μs | 31.89 μs | 1,142.2 μs | 142.5781 | 142.5781 | 142.5781 |  1,004 KB |
| **Render_Template_Beautify** | **Welcome.mjml** |   **125.1 μs** |  **0.89 μs** |  **2.49 μs** |   **124.1 μs** |  **31.7383** |   **0.2441** |        **-** |    **131 KB** |
|   Render_Template_Minify | Welcome.mjml |   126.3 μs |  1.51 μs |  4.42 μs |   125.3 μs |  31.7383 |   0.2441 |        - |    131 KB |
| Render_Template_Beautify | Worldly.mjml |   637.6 μs |  3.21 μs |  9.22 μs |   634.9 μs |  83.0078 |  83.0078 |  83.0078 |    572 KB |
|   Render_Template_Minify | Worldly.mjml |   629.6 μs |  4.24 μs | 11.60 μs |   627.9 μs |  83.0078 |  83.0078 |  83.0078 |    572 KB |



## Known Issues

### Unknown HTML Entity

We are aware with an issue where by using HTML Character Entities (e.g. `&copy;`) are unknown and throw an unhandled exception during the rendering of the MJML document. This is because we use XmlReader as the main driver for parsing the MJMl document.

The solution is to change the HTML Character Entity Names (e.g. `&copy;`) to there corresponding HTML Character Entity Number (e.g. `&#169;`) in the MJML document.
Here are some of the common HTML Character Entities:

| Result | Description                        | Entity Name | Entity Number |
| ------ | ---------------------------------- | ----------- | ------------- |
|        | non-breaking space                 | `&nbsp;`    | `&#160;`      |
| <      | less than                          | `&lt`       | `&#60;`       |
| >      | greater than                       | `&gt;`      | `&#62;`       |
| &      | ampersand                          | `&amp;`     | `&#38;`       |
| "      | double quotation mark              | `&quot;`    | `&#34;`       |
| '      | single quotation mark (apostrophe) | `&apos;`    | `&#39;`       |
| ©      | copyright                          | `&copy;`    | `&#169;`      |
| ®      | registered trademark               | `&reg;`     | `&#174;`      |
| ™      | registered trademark               | `&trade;`   | `&#8482`      |
### Non-encoded URL
We are aware of an issue with non-encoded URL's being recognized as character entities leading to an exception. This is because we use XmlReader as the main driver for parsing the MJMl document. This solution is to URL encode all of the URLs in the template.

## Contribution

We really appreciate any contribution to the project to help provide a native version of MJML to C#. Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Appreciations

Once again, it's good to share some appreciation to the projects that make `MJML.NET` possible.

- [MJML](https://github.com/mjmlio/mjml)
- [MJML.NET V1](https://github.com/LiamRiddell/MJML.NET)
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
