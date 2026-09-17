# Performance Ideas

Five improvements, each measured with a prototype against the current `perf2` branch (`c7fdbdc`). Each one saves at least 5% CPU or allocations in one of the CI benchmark scenarios. The prototypes are not part of this branch.

## Method

- A scratch harness rendered all 21 templates in `Mjml.Net.Benchmark/Templates` per round, with the same options as `CiBenchmarks`:
  - `render`: `Beautify = true`
  - `validator`: `Beautify = true` and `SoftValidator`
  - `post`: `RenderAsync` with `AngleSharpPostProcessor.Default`
- Each scenario was warmed up for 4 seconds. The base build and the prototype build then ran alternately, 25-40 blocks of 5 rounds each.
- Allocations are exact (`GC.GetAllocatedBytesForCurrentThread`) and stable within 0.2%.
- Times are noisy, because other applications kept the machine at about 50% CPU load. Time changes below about 5% are not significant.
- Rendered HTML and validation errors were compared for every template and scenario (plus `Beautify = false`), using a fixed `IIdGenerator`.
- The tests (without `ComplexTests`) pass with all prototypes combined.

Baseline per round (21 templates):

| Scenario  | Time    | Allocated |
| --------- | ------- | --------- |
| render    | ~3.0 ms | 4.58 MB   |
| validator | ~2.7 ms | 4.61 MB   |
| post      | ~27 ms  | 41.1 MB   |

The output string of `render` is ~1.07M chars (2.1 MB), so about half of the `render` allocations cannot be avoided.

The `post` time is dominated by AngleSharp, not by Mjml.Net: only 5 templates (Austin, Reactivation, Referral, UGGRoyale, Worldly) have `<style inline>`, and for them CSS parsing during the document load was the largest cost.

## Summary

| # | Idea                                                  | Scenario         | Allocated        | Time   | Output              | Status      |
| - | ----------------------------------------------------- | ---------------- | ---------------- | ------ | ------------------- | ----------- |
| 1 | Do not parse every `style` attribute during the load  | post             | **-63%**         | ~-55%  | identical           | **done**    |
| 2 | Reuse the HtmlPerformanceKit reader buffers           | render/validator | **-30%**         | ~-10%  | identical           | **done**    |
| 3 | Cache the wrapped `DeclarationInfo` objects           | post             | **-15%** (-3.7% after #1) | noise  | identical           | **done**    |
| 4 | Parse only inline style sheets, drop the tag renaming | post             | **-9%** (-25.7% after #1-#3) | ~-25% | fixes a CSS bug     | **done**    |
| 5 | Parse from the string and serialize into a pool       | post             | **-6%** (-23.0% after #1-#4) | ~-27% | identical           | **done**    |
|   | All combined                                          | render/validator | 4.58 -> 3.19 MB (-30%) | ~-10% | |             |
|   | All combined                                          | post             | 41.1 -> 8.0 MB (-80%)  | ~-70% | |             |

The percentages are for each idea applied alone to the base.

## 1. Do not parse every `style` attribute during the load (done)

**Problem:** `WithCss` registers AngleSharp.Css's internal `StyleAttributeObserver`. It parses the CSS of every `style` attribute while the document is built. The output of MJML has a `style` attribute on almost every element. The CSS parser allocates a `Stack<ushort>`, a `StringBuilderBuffer` and many `CssProperty` objects per attribute.

Loading the 5 inline templates took ~24 ms and 24 MB with CSS enabled, but only ~4.5 ms and 3.7 MB without CSS.

**Change:** The configuration of `AngleSharpPostProcessor` removes the observer from the services. The class is internal, so it is filtered by name:

```csharp
private static readonly IConfiguration HtmlConfiguration =
    new Configuration(
        Configuration.Default
            .WithCss(...)
            // ...
            .Services
            .Where(x => x.GetType().Name != StyleAttributeObserverName));
```

`ComputeExplicitStyle` still parses the `style` attribute lazily, but only for the elements that are matched by an inline rule.

`AngleSharpPostProcessorTests` checks that `WithCss` still registers a service with this name and that the configuration of the post processor does not contain it, so a rename in AngleSharp.Css fails a test instead of silently bringing back the cost.

**Result (`CiBenchmarks`, two runs against the state with idea 2):**

| Benchmark                       | Time                          | Allocated                    |
| ------------------------------- | ----------------------------- | ---------------------------- |
| Render_Templates_PostProcessors | 27.8 ms -> 12.7 ms (-54.4% / -54.9%) | 38.7 MB -> 14.4 MB (-62.8%) |
| Render_Templates                | noise (-5.8% / +1.7%)         | unchanged                    |
| Render_Templates_Validator      | noise (-3.3% / +0.7%)         | unchanged                    |

Identical output for all templates and scenarios. All tests pass.

**Risk:** The observer keeps a cached style declaration in sync after `SetAttribute("style", ...)`. The inliner writes each element only once and serialization reads the attribute, so it is not needed here. A custom `IAngleSharpPostProcessor` that reads `GetStyle()` after changing the attribute would see stale values.

## 2. Reuse the HtmlPerformanceKit reader buffers (done)

**Problem:** `new HtmlReaderWrapper(mjml)` creates a new `HtmlPerformanceKit.HtmlReader` for every render (and every `mj-include`). Each reader allocates about 67 KB before it reads the first token:

- two `CharBuffer`s with 10,240 chars (data and comment)
- several buffers with 1,024 chars (temporary buffer, attribute values)
- ~70 `Action` delegates for the states of the state machine
- a `BufferReader` with a peek queue and a `StringReader`

For 21 templates this is 1.4 MB of the 4.58 MB per round, and most of it is short-lived `char[]` garbage. Tokenizing alone (no Mjml.Net code) allocated 1.41 MB per round.

**Status:** Implemented. The fork `MjmlNet.HtmlPerformanceKit` 1.1.0 adds `HtmlReader.Reset(TextReader)` and `HtmlReader.Reset(Stream)`.

**Change:** `HtmlReaderWrapper` instances are pooled in `DefaultPools.HtmlReaders`.

- `HtmlReaderWrapper.Rent(input)` resets the pooled tokenizer to the new input.
- `Return()` resets it to `TextReader.Null`, so that the pool does not keep a reference to the last template.
- `MjmlRenderer.RenderCore` and `MjmlRenderContext.ReadFragment` return the reader when they are done.
- The `ParseError` handler is added once per wrapper and forwards to the current `OnError`.

**Result (`CiBenchmarks`, two runs against `c7fdbdc`):**

| Benchmark                       | Time            | Allocated                 |
| ------------------------------- | --------------- | ------------------------- |
| Render_Templates                | -9.3% / -12.3%  | 4.49 MB -> 3.11 MB (-30.8%) |
| Render_Templates_Validator      | -10.8% / -10.9% | 4.51 MB -> 3.13 MB (-30.6%) |
| Render_Templates_PostProcessors | noise           | 40.1 MB -> 38.7 MB (-3.5%)  |

Output identical for all templates and scenarios, including the line numbers and positions of the validation errors.

**Follow-up:** The tokenizer still reads through a `TextReader` and a peek queue. Reading from the string directly could reduce the tokenizer CPU (~15-20% of `render`).

## 3. Cache the wrapped `DeclarationInfo` objects (done)

**Problem:** `FallbackDeclarationFactory.Create` is called for every parsed CSS declaration. It wraps the default declaration into a new `DeclarationInfo` with a new `FallbackCssValueConverter` every time. For unknown properties, `DefaultDeclarationFactory` also creates new converters and `IValueConverter[]` arrays. Together, `DeclarationInfo`, `IValueConverter[]`, `StandardValueConverter`, `OrValueConverter` and `FallbackCssValueConverter` were about 6.6 MB per round.

**Change:** The declarations are immutable. `FallbackDeclarationFactory` caches them per property name in a static `ConcurrentDictionary<string, DeclarationInfo>(StringComparer.Ordinal)` and uses one static `DefaultDeclarationFactory`. The property names come from the CSS of the templates, so the cache stops adding new names at 1,024 entries.

**Result (prototype on the original base):** 41.1 MB -> 35.0 MB (-14.7%). Time within noise.

**Result (implemented on top of #1 and #2):**

| Benchmark                       | Time  | Allocated                  |
| ------------------------------- | ----- | -------------------------- |
| Render_Templates_PostProcessors | noise | 14.38 MB -> 13.85 MB (-3.7%) |

Idea #1 removed the parsing of the `style` attributes, which created most of the declarations, so the cache saves much less than in the prototype. Four `CiBenchmarks` runs and an interleaved harness run showed no time difference beyond the noise of the machine. Identical output.

## 4. Parse only inline style sheets, drop the tag renaming (done)

**Problem:**
- AngleSharp parses every `<style>` element into a style sheet while loading the document, including the large media query and reset styles in the head of every MJML document. The inliner only needs the `<style inline>` sheets.
- To exclude the other sheets, `InlineCssPostProcessor` renames each of them to `non_inline_style` and back. Each rename creates a new element and copies `InnerHtml`, so the CSS is serialized and parsed as HTML twice. When the element becomes a `<style>` again, its CSS is parsed a second time.

**Change:** `InlineOnlyStylingService` replaces `IStylingService` and wraps `CssStylingService`. It returns no style sheet when the owner element has no `inline` attribute. Only inline sheets are then visible in `GetStyleCollection`, so the renaming in `InlineCssPostProcessor` was removed.

```csharp
public Task<IStyleSheet> ParseStylesheetAsync(IResponse response, StyleOptions options, CancellationToken cancel)
{
    if (options.Element?.HasAttribute("inline") != true)
    {
        return Task.FromResult<IStyleSheet>(null!);
    }

    return inner.ParseStylesheetAsync(response, options, cancel);
}
```

**Result (prototype on the original base):**
- Styling service alone: 41.1 MB -> 38.4 MB (-6.5%), time about -9%.
- With the renaming removed as well: 37.2 MB (-9.4%), time about -10%.

**Result (implemented on top of #1-#3, `CiBenchmarks`, two runs):**

| Benchmark                       | Time            | Allocated                     |
| ------------------------------- | --------------- | ----------------------------- |
| Render_Templates_PostProcessors | -26.3% / -25.2% | 13.85 MB -> 10.29 MB (-25.7%) |

**Bug fix:** The renaming escaped `>` in the CSS of non-inline styles, which broke child combinators. Worldly rendered `.mj-menu-checkbox[type="checkbox"] ~ .mj-inline-links &gt; a { ... }` and now renders the correct `> a`. This is the only output difference. `StyleTests.Should_keep_non_inline_styles_when_inlining` covers it and failed with the old code.

**Risks:** A custom `IAngleSharpPostProcessor` that reads the non-inline style sheets would no longer see them. Only use the service when all inner processors do not need them, or make it an option of `AngleSharpPostProcessor`.

## 5. Parse from the string and serialize into a pool (done)

**Problem:**
- `context.OpenAsync(req => req.Content(html))` goes through the navigation pipeline. The HTML string is encoded to UTF-8 into a `MemoryStream` (`Byte[]` ~750 KB per round), then decoded again into a `StringBuilder`-backed text source.
- `document.ToHtml()` writes into a new `StringWriter`, which grows its `StringBuilder` chunk by chunk for the whole document.

**Change:**

```csharp
using var document = await context.GetService<IHtmlParser>()!.ParseDocumentAsync(html, ct);

// ...

var sb = Writers.Get();
try
{
    using (var writer = new StringWriter(sb))
    {
        document.ToHtml(writer, HtmlMarkupFormatter.Instance);
    }

    return sb.ToString();
}
finally
{
    Writers.Return(sb);
}
```

`Writers` is a `DefaultObjectPool<StringBuilder>` with a `MaximumRetainedCapacity` of 256K chars, like `DefaultPools.StringBuilders`. Style sheets and the context services work as before, because the parser is bound to the pooled browsing context.

**Result (prototype on the original base):** 41.1 MB -> 38.5 MB (-6.3%), time about -4%. Direct parsing alone was -3.9%.

**Result (implemented on top of #1-#4, `CiBenchmarks`, two runs):**

| Benchmark                       | Time            | Allocated                    |
| ------------------------------- | --------------- | ---------------------------- |
| Render_Templates_PostProcessors | -25.0% / -29.0% | 10.29 MB -> 7.92 MB (-23.0%) |

Identical output.

## Measured, but below 5%

- **`IndexOf('\n')` in `InnerTextOrHtml.AppendIntended`** instead of a loop over every char (**done**): identical output, no allocation change. `CiBenchmarks` showed +4.1% / -3.4% for `Render_Templates` (noise). An interleaved harness run showed about -1% (3.52 ms -> 3.48 ms).
- **Concrete `Dictionary` enumeration in the generated `Bind` method** (no boxed `IReadOnlyDictionary` enumerator), plus direct component factories instead of `new T()`: -1.1% allocations, time within noise.
- **GC settings** (`GCRetainVM`, larger gen0): no effect.
- **Index `AttributesByClass` by class name** in `Binder.Resolve`: the current code is quadratic in the number of `mj-class` attributes, but only UGGRoyale and Worldly use `mj-class`, so the benchmarks would not show it.
