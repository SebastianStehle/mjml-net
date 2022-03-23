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
## Benchmarks
Performance was one of the key focuses for this project. We're aiming to support high
througput while mainintaing low memory footprint. Below are the benchmarks for every public MJML template compiled (beautified and minified). 

**Important: These tests do not include loading or saving the template from/to disk.**

If you'd like to run the benchmarks your self then you can run the `Mjml.Net.Benchmarks` project in `release` mode.

### Benchmark Specs
```
* BenchmarkDotNet=v0.13.1, 
* OS=Windows 10.0.19043.1586 (21H1/May2021Update)
* Intel Core i7-4790 CPU 3.60GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
* .NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
```
### Benchmark Key
```
Mean   : Arithmetic mean of all measurements
Error  : Half of 99.9% confidence interval
StdDev : Standard deviation of all measurements
Median : Value separating the higher half of all measurements (50th percentile)
1 us   : 1 Microsecond (0.000001 sec)
```
### Benchmark Results
|                                  Method |     Mean |    Error |   StdDev |   Median |
|---------------------------------------- |---------:|---------:|---------:|---------:|
|      Render_Template_Arturia_Beautified |       NA |       NA |       NA |       NA |
|       Render_Template_Austin_Beautified | 298.0 us |  5.92 us |  9.38 us | 295.6 us |
|  Render_Template_BlackFriday_Beautified | 199.7 us |  5.56 us | 16.23 us | 196.0 us |
|         Render_Template_Card_Beautified | 284.4 us |  5.68 us | 11.48 us | 281.8 us |
|    Render_Template_Christmas_Beautified | 407.6 us |  7.90 us | 20.25 us | 401.1 us |
| Render_Template_HappyNewYEar_Beautified | 152.1 us |  3.01 us |  5.73 us | 150.6 us |
|      Render_Template_OnePage_Beautified | 435.9 us |  8.65 us | 21.71 us | 431.5 us |
|        Render_Template_Proof_Beautified | 203.6 us |  4.22 us | 12.39 us | 201.9 us |
|       Render_Template_Racoon_Beautified | 955.7 us | 27.12 us | 79.11 us | 924.6 us |
| Render_Template_Reactivation_Beautified | 266.5 us |  5.16 us | 11.97 us | 264.1 us |
|   Render_Template_RealEstate_Beautified | 729.5 us | 14.23 us | 23.37 us | 728.7 us |
|       Render_Template_Recast_Beautified |       NA |       NA |       NA |       NA |
|      Render_Template_Receipt_Beautified | 288.7 us |  5.36 us | 10.06 us | 287.8 us |
|     Render_Template_Referral_Beautified | 140.7 us |  2.77 us |  5.59 us | 139.1 us |
| Render_Template_SpheroDroids_Beautified |       NA |       NA |       NA |       NA |
|   Render_Template_SpheroMini_Beautified | 420.2 us |  7.91 us | 21.39 us | 415.0 us |
|    Render_Template_UggRoyale_Beautified |       NA |       NA |       NA |       NA |
|      Render_Template_Welcome_Beautified | 131.0 us |  2.59 us |  5.51 us | 129.6 us |
|      Render_Template_Worldly_Beautified | 584.1 us | 11.50 us | 11.81 us | 580.3 us |
|        Render_Template_Arturia_Minified |       NA |       NA |       NA |       NA |
|         Render_Template_Austin_Minified | 283.1 us |  5.52 us |  9.95 us | 282.4 us |
|    Render_Template_BlackFriday_Minified | 181.5 us |  3.43 us |  3.21 us | 182.1 us |
|           Render_Template_Card_Minified | 285.9 us |  5.70 us | 15.13 us | 282.6 us |
|      Render_Template_Christmas_Minified | 410.4 us |  8.04 us | 14.29 us | 407.4 us |
|   Render_Template_HappyNewYEar_Minified | 156.6 us |  2.97 us |  6.40 us | 155.7 us |
|        Render_Template_OnePage_Minified | 414.5 us |  8.11 us |  9.96 us | 412.5 us |
|          Render_Template_Proof_Minified | 189.9 us |  3.73 us |  3.11 us | 188.1 us |
|         Render_Template_Racoon_Minified | 909.5 us | 18.07 us | 29.69 us | 912.7 us |
|   Render_Template_Reactivation_Minified | 261.3 us |  4.92 us | 11.32 us | 257.6 us |
|     Render_Template_RealEstate_Minified | 697.7 us |  6.38 us |  5.33 us | 698.0 us |
|         Render_Template_Recast_Minified |       NA |       NA |       NA |       NA |
|        Render_Template_Receipt_Minified | 277.3 us |  5.45 us |  8.16 us | 275.1 us |
|       Render_Template_Referral_Minified | 151.0 us |  3.08 us |  9.04 us | 150.3 us |
|   Render_Template_SpheroDroids_Minified |       NA |       NA |       NA |       NA |
|     Render_Template_SpheroMini_Minified | 395.4 us |  6.26 us |  5.55 us | 393.6 us |
|      Render_Template_UggRoyale_Minified |       NA |       NA |       NA |       NA |
|        Render_Template_Welcome_Minified | 131.9 us |  2.65 us |  7.74 us | 130.2 us |
|        Render_Template_Worldly_Minified | 597.1 us | 10.85 us | 30.42 us | 583.7 us |



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

### Non-encoded URL
We are aware of an issue with non-encoded URL's being recognized as character entities leading to an exception. This is because we use XmlReader as the main driver for parsing the MJMl document. This solution is to URL encode all of the URLs in the template.

## Contribution

We really appreciate any contribution to the project to help provide a native version of MJML to C#. Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Appreciations

Once again, it's good to share some appreciation to the projects that make `MJML.NET` possible.

- [MJML](https://github.com/mjmlio/mjml)
- [MJML.NET V1](https://github.com/LiamRiddell/MJML.NET)
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
