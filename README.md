# MJML.NET 2 

An unofficial port of [MJML](https://mjml.io/) (by [MailJet](https://www.mailjet.com/)) to .NET Core.

This project is currently in an **experimental state** and should not be used in a production environment.

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
PM > Install-Package MjmlDotNet2
```

## Known Issues

### Unknown HTML Entity

We are aware with an issue where by using HTML Character Entities (e.g. `&copy;`) are unknown and throw an unhandled exception during the rendering of the MJML document. This is because
we use XmlReader as the main driver for parsing the MJMl document.

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

## Contribution

We really appreciate any contribution to the project to help provide a native version of MJML to C#.

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Side Note: In your PR you should summarise your changes, bug fixes or general modifications.

## Appreciations

Once again, it's good to share some appreciation to the projects that make `MJML.NET V2` possible.

- [MJML](https://github.com/mjmlio/mjml)
- [MJML.NET V1](https://github.com/LiamRiddell/MJML.NET)
- [AngleSharp](https://github.com/AngleSharp/AngleSharp)
