# Mjml.Net

Unofficial .NET port of [MJML](https://mjml.io/). Renders MJML templates to responsive email HTML.

## Solution

- .NET solution `Mjml.Net.slnx`:
  - `Mjml.Net/` — the renderer: reading (HtmlPerformanceKit), components under `Components/`, helpers, types and validators.
  - `Mjml.Net.Generator/` — source generator for the `[Bind]` fields of components. The generated code comes from `Template.handlebar`.
  - `Mjml.Net.PostProcessors/` — optional AngleSharp post processors (inline CSS, `mj-html-attributes`), only used by `RenderAsync`.
  - `Mjml.Net.Benchmark/` — BenchmarkDotNet benchmarks with the templates under `Templates/`.
  - `Tests/` — xUnit tests with expected outputs under `Components/Outputs`.
  - `Tools/` — helpers to port components from the MJML sources.
- Rendering pipeline: `MjmlRenderer` → `MjmlRenderContext.Read` (builds the component tree) → `Bind` (generated code, attributes from `Binder`) → `Measure` → `Render` into a pooled `RenderBuffer` → helpers.

## API stability

- Only `IMjmlRenderer` is a stable public API and must stay backwards compatible (use default interface methods for new members).
- Everything else (components, `IComponent`, `IBinder`, `GlobalContext`, renderer interfaces, post processor interfaces, generated code, ...) is allowed to change, even if it is public.

## Tests

Some tests compare the output with the official MJML compiler and need Node.js (`npx mjml`). Run the tests like this to skip them:

```bash
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName!~Tests.ComplexTests&FullyQualifiedName!=Tests.IncludeTests.Should_render"
```

- Use xUnit assertions (`Assert.Equal`, `Assert.True`, `Assert.Single`, `Assert.Empty`, ...) for simple properties and values.
- Use FluentAssertions (`Should().BeEquivalentTo(...)`) only for deep, structural comparisons of objects and collections.
- Compare rendered HTML with `AssertHelpers.HtmlFileAssert` against a file in `Components/Outputs`. Only regenerate an expected output after checking the difference.

## Benchmarks

```bash
dotnet run -c Release --project Mjml.Net.Benchmark
```

```bash
dotnet run -c Release --project Mjml.Net.Benchmark -- --postprocessors
```

- The template benchmarks compare the current source with older NuGet versions (configurations `V1_24`, `V2_0`, `V2_1`, `V3_8`). Code in the benchmark project must compile against these versions.

## Performance

- Measure before and after an optimization. Do not rely on assumptions or a sampling profiler alone.
- Rendering changes must not change the output. Compare the rendered HTML of the benchmark templates (with a fixed `IIdGenerator`) before and after the change.

## Best Practices

- Code style is enforced by StyleCop (`stylecop.json`) and `.editorconfig`. The build must not have new warnings. Follow the surrounding file's conventions, including encoding and line endings.
- Do not write XML comments.
- Exception: `IMjmlRenderer` keeps its XML comments, because it is the documented public API.
- Do write precise short comments and only when needed.
- Do not comment a class or a method, only put comments inside functions or above variables.
- Do not comment properties in general. Only comment behavior inside classes.

## Maintaining this file

- When the user states a new general coding guideline, confirm it with the user and then add it to this file automatically.
