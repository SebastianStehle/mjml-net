# Performance Analysis – Mjml.Net

I found these by reading the code. I did **not** run benchmarks to measure them. To check the effect of any change, run `Mjml.Net.Benchmark` (`TemplateBenchmarks`, 21 templates, `[MemoryDiagnoser]`) before and after the change.

## Result: `main` vs. current version

BenchmarkDotNet (MediumRun, Release, .NET 10); each operation renders all 21 benchmark templates with beautify:

| Scenario | `main` | Current | Time | Allocated |
|---|---|---|---|---|
| `Render` | 4.81 ms / 10.77 MB | 2.78 ms / 4.48 MB | −42% | −58% |
| `Render` with `SoftValidator` | 6.70 ms / 15.45 MB | 2.91 ms / 4.51 MB | −57% | −71% |
| `RenderAsync` with `AngleSharpPostProcessor.Default` | 1,353 ms / 489.78 MB | 40.5 ms / 40.03 MB | −97% (33×) | −92% |
| `Render` into a `TextWriter` (new) | – | 2.03 ms / 2.43 MB | −58% | −77% |

CPU time measured separately as process CPU time including GC threads, 3 alternating rounds of 31,500 renders each:

| Per render | `main` | Current | Change |
|---|---|---|---|
| CPU time | ~368 µs | ~231 µs | −37% |
| Allocated | ~523 KB | 218 KB | −58% |
| GCs (gen0 / gen2) | 2,062 / 1,406 | 807 / 807 | −61% / −43% |

With post-processors, the output differs for templates with inline styles, because the current version only inlines like mjml (#11).

## How rendering works (hot path)

`MjmlRenderer.RenderCore` → `MjmlRenderContext.Read` (HtmlPerformanceKit tokenizer wrapped by `HtmlReaderWrapper`/`SubtreeReader`) → for each tag: `CreateComponent` + pooled `Binder` → `Bind` (generated code: one `Binder.GetAttribute` + `Coerce` per `[Bind]` field) → `Measure` → `Render` into a pooled `StringBuilder` (`RenderBuffer`) → helpers (`StyleHelper`, `FontHelper`, …) → `ToText()`. The async path can also run AngleSharp post-processors, which parse the whole document again.

Allocation matters most here. On a typical template, most of the cost is attribute resolution during bind, string building during render, and short-lived objects created for every component.

---

## TODOs

### 1. ✅ Fix O(n²) sibling counting in `Component.MeasureChildren`
[Mjml.Net/Component.cs:132](Mjml.Net/Component.cs#L132)

```csharp
child.Measure(context, width, childNodes.Count, childNodes.Count(x => !x.Raw));
```

For every child, this runs a LINQ `Count` over all siblings again. That is O(n²) per parent, and it allocates an enumerator and a closure every time. Count the non-raw children once before the loop. This path runs for every container: body, section, column, group, hero, and wrapper.

### 2. ✅ Keep large `StringBuilder`s in the pool
[Mjml.Net/DefaultPools.cs:11](Mjml.Net/DefaultPools.cs#L11)

`StringBuilderPooledObjectPolicy` has `MaximumRetainedCapacity = 4 * 1024` by default. A rendered email is usually 20–100 KB, so the pool throws away the main output builder after every render, and the next render grows a new builder from scratch through many chunk allocations (LOH for big mails). Fix options:
- Use a separate policy with a higher `MaximumRetainedCapacity`, such as 256 KB, for the root buffer.
- Pre-size the root buffer from the input length, such as `mjml.Length * 3`.

**✅ Follow-up: buffer leak.** Raising the limit only helped partly. An allocation sample still showed `char[]` and `StringBuilder` objects making up about 55% of the allocated bytes, even after more than 1,000 warm-up renders. The cause was in `RenderBuffer.Plain(IBuffer)`: it returned early for empty buffers without disposing them, so their builders never went back to the pool. A template with an empty `mj-head` lost one builder on every render. The pool then created fresh builders, and those had to grow to the full output size again. Empty buffers are now disposed as well.

| | Before | After |
|---|---|---|
| Allocated per render, all 21 templates interleaved | 465 KB | 259 KB |
| BenchmarkDotNet, the 8 templates that use `mj-attributes` | 3.14 ms / 5.45 MB | ~2 ms (noisy) / 2.26 MB |

(I first suspected the pool's rotating order and gave each render context its own builders. That version measured exactly the same as the one-line fix, so I dropped it.)

### 3. ✅ Stop allocating in `AllowedFields` on every access (generated code)
[Mjml.Net.Generator/Template.handlebar](Mjml.Net.Generator/Template.handlebar)

`BodyComponentBase` declares `[Bind("css-class")]`, so `base.AllowedFields` is non-empty for every body component. As a result, the generated getter builds a **new `AllowedAttributes` dictionary and copies it** every time the property is read. `ValidatorBase` reads it for each component and attribute. Merge the inherited entries once, in the static constructor or through a `Lazy`, and return that cached instance.

### 4. Short-circuit `Binder.GetAttribute` for the common "not set" case
[Mjml.Net/Internal/Binder.cs:81](Mjml.Net/Internal/Binder.cs#L81)

The generated `Bind()` calls `GetAttribute` for **every** bindable field. `ColumnComponent` has about 25 of them, and most are never set. Each call that finds nothing goes through:
- a local dictionary lookup
- a virtual `GetInheritingAttribute` call on the parent
- class and parent-class lookups
- two lookups in `AttributesByName` (the element name, then `mj-all`)

Ideas:
- Precompute the `mj-attributes` defaults once per element name (element defaults merged with `mj-all`) into one dictionary, or skip that stage entirely when no `mj-attributes` exist.
- Only take the class-lookup path when the element actually has `mj-class`.
- Look up `ClassNames` for the parent once per binder instead of once per attribute.

**❌ Tried and reverted.** I added a per-element index (`element → name → value`) in `GlobalContext` and had each binder look up the element's and `mj-all`'s dictionaries once. With BenchmarkDotNet (MediumRun, the 8 templates that use `mj-attributes`) it made no difference: 3.20 ± 0.32 ms with the change against 3.14 ± 0.45 ms without, and allocations were the same. When a template has no `mj-attributes`, the lookups already cost almost nothing, because an empty `Dictionary` returns before hashing. The other checks were already guarded by `Count > 0`, so attribute resolution is not a bottleneck.

### 5. Replace `GlobalData` scans with typed storage
[Mjml.Net/GlobalContext.cs](Mjml.Net/GlobalContext.cs), [Helpers/Style.cs](Mjml.Net/Helpers/Style.cs), [Components/Body/BodyComponent.cs:39-47](Mjml.Net/Components/Body/BodyComponent.cs#L39)

`GlobalData` is a `Dictionary<(Type, object), GlobalData>`, and it has these costs:
- The key is `object`, so string and Guid identifiers get boxed.
- `AddGlobalData` calls `Guid.NewGuid()` for every item.
- The code repeatedly runs `GlobalData.Values.OfType<T>()` over the whole dictionary:
  - `StyleHelper` scans three times for `MediaQuery` and once for `Style`.
  - `BodyComponent` scans four times for `Title`, `Direction`, and `Language`, and scans for `Title` twice.
  - `RootComponent`, `PreviewHelper`, and `TitleHelper` also scan.

Store data per type instead, such as `Dictionary<Type, IList>` or a generic static-slot pattern, and use an incrementing counter instead of `Guid`.

**⏭️ Measured, not worth it.** I measured this with BenchmarkDotNet, using a `GlobalContext` with 20 entries (12 media queries, 4 styles, title, language, direction and font) and running the scans a single render does. The scans took about 1.2 µs and allocated 968 B, and three `Guid` keys took about 0.2 µs and allocated 96 B. That is about 0.35% of a render (~0.4 ms) and about 0.2% of its allocations (~465 KB), too little to show up in the template benchmarks. `GlobalContext.GlobalData` is also a public `Dictionary<(Type, object), GlobalData>`, so switching to typed storage would break the public API.

### 6. Avoid string allocations in the hot rendering helpers
- `WriterExtensions.StyleIfNumber`, `StyleIf`, and `AttrOrAuto` ([Extensions/WriterExtensions.cs](Mjml.Net/Extensions/WriterExtensions.cs)) use `$"{value}{unit}"`, which builds a string only to append it. Route these through the existing interpolated-handler overloads, such as `Style(string, ref StyleInterpolatedStringHandler)`, so they write straight into the buffer.
- `ColumnComponent.Measure` builds `FormattableString.Invariant($"{widthValue}%")` for every column. `MediaQuery.Width` does the same for each media query.
- `ColorType.Coerce` allocates a `char[]` and then a `string`. Use `string.Create`.
- `Style.Static` allocates a closure for each style. Make it a subclass or a static lambda with state.

**⏭️ Measured, not worth it.**
- **The `WriterExtensions` point was wrong.** C# prefers an interpolated string handler overload over a `string` overload, so `renderer.Style(name, $"{value}{unit}")` already writes straight into the buffer without allocating.
- **Columns are the largest remaining item, and they are small.** The width string, class name and `MediaQuery` of one column allocate 488 B. The benchmark templates average 12 columns, which comes to about 5.7 KB per render, or about 2% of the ~258 KB a render allocates. `ColorType.Coerce` only runs for 3-digit hex colors, and `Style.Static` only runs for `mj-include` CSS.
- **Where the ~258 KB per render goes now:** about 100 KB is the returned HTML string, which can't be avoided. About 53 KB is the up-front buffers of each new `HtmlPerformanceKit.HtmlReader` (two 10,240-char buffers plus smaller ones), and the library has no API to reuse a reader. About 28 KB is parsed tag, attribute and text strings, and about 30 KB is strings created while rendering.

### 7. Parse shorthand values without `Split`
[Mjml.Net/BindingHelper.cs:34](Mjml.Net/BindingHelper.cs#L34), [SectionComponent.cs:357,494](Mjml.Net/Components/Body/SectionComponent.cs#L357), [MsoButtonComponent.cs:46](Mjml.Net/Components/Body/MsoButtonComponent.cs#L46), [Binder.ClassNames](Mjml.Net/Internal/Binder.cs#L22)

`ParseShorthandValue` runs for every `padding` or `border-radius` shorthand in generated `Bind()`. It allocates a `string[]` and substrings. For the common 1-part case (`padding="10px"`), return the original string with no allocation. For 2–4 parts, scan with `ReadOnlySpan<char>.IndexOf(' ')`. The `mj-class` split and the background-position split can use the same approach.

**⏭️ Measured, not worth it.** Across all `Split` calls in the library, `string[]` allocations add up to about 5.6 KB per render, about 2% of the total. The substrings would still be allocated, because they are stored in fields. `ParseShorthandValue("10px 25px")` takes 38 ns and allocates 104 B. For one value, `Split` already returns the original string without a substring, so the only saving would be the array.

### 8. Parse each unit value once, not in every `Measure`/`Render`
[ColumnComponent.cs:120](Mjml.Net/Components/Body/ColumnComponent.cs#L120), [SectionComponent.cs:94](Mjml.Net/Components/Body/SectionComponent.cs#L94), [ImageComponent.cs:109](Mjml.Net/Components/Body/ImageComponent.cs#L109), [HeroComponent.cs](Mjml.Net/Components/Body/HeroComponent.cs), [ButtonComponent.cs:197](Mjml.Net/Components/Body/ButtonComponent.cs#L197)

`UnitParser.Parse` runs 4–6 times per component, and often on `null`. It is cheap, but it runs very often. Three improvements:
- Return early on `null` before `IsNullOrWhiteSpace` and `Trim`.
- Parse `px` integers by hand instead of calling `int.TryParse` with `NumberStyles.Any` and a culture.
- Cache parsed paddings and borders on the component in `Bind`.

Also, `char.IsNumber` accepts Unicode digits. `char.IsAsciiDigit` is faster and correct here.

**⏭️ Measured, not worth it.** `UnitParser.Parse` doesn't allocate. It takes 4 ns for `null`, 37 ns for `"25px"` and 74 ns for `"33.33333333333333%"`. There are about 40 call sites, which comes to a few hundred calls per render and most of those are on `null`. In the worst case that is well under 10 µs of a ~250–400 µs render.

### 9. Reduce per-render overhead in the render pipeline
- `RenderCore` creates a `new MjmlOptions()` record on every call when `options` is null. Use a static default instance. `MjmlRenderContext.Setup` does the same thing again.
- `Render(TextReader)` and `RenderAsync(Stream)` read the whole input into a `string`, and then `HtmlReaderWrapper` wraps it again in a `StringReader`. Pass the `TextReader` straight to `HtmlReader`.
- `StartBuffer` allocates a `new RenderBuffer` class for each buffer. Nested buffers are used for conditional or MSO sections, so pool these as well.
- `BindComponentAttributes` creates a `SourcePosition` for every attribute even when `Validator` is null. Guard it with `if (mjmlOptions.Validator != null)`.
- `ReadElement` looks up the component factory in a `Dictionary<string, Func<IComponent>>` and then calls a delegate. `new T()` in a generic lambda goes through `Activator`. Use a generated `switch` or a `FrozenDictionary` of static factory lambdas.
- `RenderBuffer.WriteLineStart` and `InnerTextOrHtml.WriteLineStart` append spaces in a loop. Use `sb.Append(' ', indent * 2)`.
- `Validate()` copies the error list even when it is empty.

**Partly done.**
- **✅ Indentation:** `RenderBuffer.WriteLineStart` and `InnerTextOrHtml.WriteLineStart` now use `sb.Append(' ', count)`. A beautified render writes about 800 lines with an average of 24 indentation characters. The loop took 15.2 ns per line and the single call takes 2.4 ns, which saves about 10 µs per render (~3%). The output is byte-for-byte identical for all 21 templates, with and without beautify.
- **❌ Static default `MjmlOptions`: not safe.** `BreakpointComponent` writes `context.Options.Breakpoint`, so a shared instance would carry one template's `mj-breakpoint` into later renders. The same line also changes the `MjmlOptions` instance passed in by the caller.
- **⏭️ Skipped the rest.** Each remaining item is a single small allocation or a few nanoseconds per render or per component.

### 10. Make the AngleSharp post-processing path cheaper
[Mjml.Net.PostProcessors/InlineCssPostProcessor.cs](Mjml.Net.PostProcessors/InlineCssPostProcessor.cs), [AngleSharpPostProcessor.cs](Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs)

When post-processors are on, this is probably the most expensive part of rendering, though I have not measured it. The code already reuses the parsed document across inner processors, but:
- `ProcessAsync` walks the full DOM **three times**, and `Traverse` allocates a `ToList()` copy of the children at every node.
- `InlineStyle` calls `document.Context.GetService<IRenderDevice>()` and `view.GetStyleCollection(device)` **for every element**. Build the style collection once per document.
- `RenameTag` clones `<style>` elements by serializing and re-parsing `InnerHtml`. Instead, record the non-inline style elements, disable them temporarily (for example by moving them out of the tree), and put them back afterwards.
- `BrowsingContext.New` runs for every render. Check whether a context can be reused.
- Add a benchmark that runs `RenderAsync` with `AngleSharpPostProcessor.Default`. The current benchmark only covers the sync path.

**✅ Done. This was by far the most expensive path.**
- **Before:** with `AngleSharpPostProcessor.Default`, a render took about 128 ms and allocated 23.6 MB, against 0.6 ms and 260 KB without post-processors.
- **Where the time went:** almost all of it was inline CSS. A sampling profile showed that `GetDeclarations` went through AngleSharp.Css's internal `StyleCollection`, which enumerates every style sheet and rule again for each element and for each of its ancestors.
- **The fix:** `InlineCssPostProcessor` now collects the rules once per document into a `CachedStyleCollection` (a list behind the public `IStyleCollection` interface). This is safe because inlining doesn't change the style sheets.
- **Result:** about 27 ms and 16.7 MB per render, roughly 4.7× faster. The output is byte-for-byte identical for all 21 benchmark templates (5 of them use `mj-style inline="inline"`), and all 13 post-processor tests pass.
- **What's left is inside AngleSharp:** it parses every `style` attribute while loading the document, parses the written CSS again on `SetAttribute` through its attribute observer, and computes cascaded styles.
- **Tried and dropped:**
  - Running the full cascade only for elements that inline rules match. It was 7.6× faster but changed the output, because the current code also copies inherited properties such as `word-spacing` onto every descendant.
  - Caching `FallbackDeclarationFactory.Create`. It is called about 16,000 times per render, but caching made no difference to time.
- **Benchmark:** `PostProcessorBenchmarks` runs `RenderAsync` with the post-processors, using `dotnet run -c Release -- --postprocessors`. It is only built for Debug and Release, because the old packages don't include the post-processors.

---

## More TODOs (measured on the current state)

Items 1–10 started as guesses from reading the code. These five come from measurements taken after the fixes above: allocation sampling by type, GC counts, and a phase-by-phase timing of the post-processing. The numbers are averages per render over the 21 benchmark templates (Release, .NET 10). This machine is noisy, so treat timings as rough.

With post-processors, a render currently spends about **13 ms parsing** (5.6 MB), **22 ms inlining CSS** (10.3 MB), and **0.9 ms in `ToHtml`**. Without post-processors, a render allocates **~258 KB**.

### 11. Only inline CSS when there is something to inline
[Mjml.Net.PostProcessors/InlineCssPostProcessor.cs](Mjml.Net.PostProcessors/InlineCssPostProcessor.cs)

**Potential: the entire ~36 ms of post-processing for most templates.**
- **No inline styles in most templates:** 16 of the 21 templates have no `<mj-style inline="inline">`, and none uses `mj-html-attributes`. The processor still parses the whole document with AngleSharp and computes the cascade for all ~255 elements.
- **Rewrites every `style` attribute:** in that case it normalizes each attribute (`color:red;` → `color: red`) and copies inherited properties onto every descendant. For example, `word-spacing: normal` from `<body>` shows up on each `tbody`, `tr`, `td` and so on. As far as I know, the official mjml only runs its CSS inliner (juice) when there are inline styles, and juice only applies rules that match, so this output probably differs from mjml's.
- **What to decide:** is the normalization and the copying of inherited properties intended? If not:
  - skip the inline step, and the AngleSharp parse when no other post-processor needs it, whenever no `<style inline>` exists;
  - otherwise, compute the cascade only for elements that inline rules match. I prototyped this: 7.6× faster, and the only output change was the copied inherited properties.
- **Verify** against the reference output with `npx mjml`, which is what the `ComplexTests` already do.

**✅ Done: the behavior now matches mjml.** The mjml source (`mjml-core`) only calls juice `if (globalData.inlineStyle.length > 0)`, with `applyStyleTags: false`.
- **`IAngleSharpPostProcessor.ShouldProcess(string html)`:** a new default interface method, so existing implementations still compile. `AngleSharpPostProcessor` returns the HTML unchanged, without parsing or serializing, when no inner processor needs it. `InlineCssPostProcessor` looks for a `<style …>` tag with `inline`. `AttributesPostProcessor` looks for `mj-html-attribute` or `mj-selector`.
- **Only matched elements:** the inline step runs `ComputeExplicitStyle` (matched rules plus the element's own `style`, without inheritance) only for elements that the rules of the inline style sheets match. Other `style` attributes are left as written.
- **Result:** with `AngleSharpPostProcessor.Default`, a render takes **~4 ms and ~2 MB**, down from ~27 ms / 16.7 MB (and from 128 ms / 23.6 MB originally). The 16 templates without inline styles come out the same as a normal render; only the random navbar IDs differ.
- **Found along the way, fixed:** `FallbackDeclarationFactory` wrapped the converters of shorthand properties as well, and AngleSharp drops shorthands whose converter it doesn't recognize. As a result, `padding`, `margin`, `background`, `border`, `border-radius` and `text-decoration` disappeared from every element the inliner touched. Before this change, that meant every element of every post-processed document. Shorthands are no longer wrapped. `StyleTests.Should_render_inline4_with_shorthand` now uses an inline rule, and I regenerated its fixture. It checks that the button link keeps all its shorthands and gets the inlined `letter-spacing`.

### 12. Stop AngleSharp from re-parsing the CSS we just wrote
[Mjml.Net.PostProcessors/InlineCssPostProcessor.cs](Mjml.Net.PostProcessors/InlineCssPostProcessor.cs)

**Potential: ~7.3 ms and ~3.7 MB per render, about 25% of post-processing.**
- **The cost:** `element.SetAttribute("style", css)` triggers AngleSharp.Css's `StyleAttributeObserver`, which parses the CSS string again into the element's style declaration. The declaration is thrown away, because we only serialize afterwards. Timed separately, the `SetAttribute` calls alone take 7.3 ms and allocate 3.7 MB.
- **Idea:** compute all declarations first. This is equivalent, because inlining already runs children before parents and doesn't read computed children. Keep the CSS strings in a `Dictionary<IElement, string>`, and emit them from a custom `IMarkupFormatter` during `ToHtml` instead of calling `SetAttribute`.

**⏭️ No longer worth it after #11.** Only elements matched by inline rules get `SetAttribute` now. On the 5 templates with inline styles, the whole inline step takes ~3.9 ms and 817 KB, so the re-parse is a small part of that. Parsing the document dominates.

### 13. Reuse the AngleSharp `BrowsingContext`
[Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs:72](Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs#L72)

**Potential: ~1 ms and ~387 KB per render.**
- **The cost:** `BrowsingContext.New(HtmlConfiguration)` runs on every render. Creating a context alone takes about 1 ms and allocates 387 KB (services, factories, entity provider).
- **Idea:** a context can open many documents, but it isn't thread-safe. Pool contexts with an `ObjectPool<IBrowsingContext>`, and dispose or close each document after `ToHtml`.
- **Check:** confirm that nothing accumulates in a reused context, such as history or the active document.

**✅ Done.** `AngleSharpPostProcessor` takes contexts from a `DefaultObjectPool<IBrowsingContext>` and disposes each document after `ToHtml`.
- **Reuse is safe:** a reused context produced identical output, its retained heap didn't grow after 300 documents, and it has no session history.
- **Concurrency is safe:** with a fixed ID generator, 400 parallel renders matched the sequential output. Without it, only the random carousel and navbar IDs differ.
- **Saved:** ~500 KB per processed document.

### 14. Don't create the full HTML string when the caller doesn't need it
[Mjml.Net/MjmlRenderer.cs:195](Mjml.Net/MjmlRenderer.cs#L195), [RenderBuffer.cs](Mjml.Net/RenderBuffer.cs)

**Potential: ~100 KB per render (~39% of what remains), and most gen2 GCs.**
- **Every GC is gen2:** over 6,300 renders, gen0/gen1/gen2 collections were 161/161/161. 57% of the returned HTML strings are at least 85 KB, so they are allocated on the large-object heap, and those allocations are what trigger the full collections. On a server with a large heap, gen2 GCs are the expensive kind.
- **The result string is the largest remaining allocation**, at about 100 KB per render.
- **Idea:** add overloads such as `Render(string mjml, TextWriter output, MjmlOptions?)` and `RenderAsync(..., Stream output, ...)`. They would write the `StringBuilder` chunks (`GetChunks()`) directly, for example into an ASP.NET response or an email library.
- **With post-processors:** the HTML is materialized as a string first and then parsed. Passing AngleSharp the buffer content instead would avoid one full copy.

**✅ Done (sync).**
- **New overload:** `ValidationErrors Render(string mjml, TextWriter output, MjmlOptions? options = null)` on `IMjmlRenderer`, as a default interface method that falls back to the string version, and implemented in `MjmlRenderer`. It writes the `StringBuilder` chunks straight into the writer.
- **Result:** writing into a reused `StreamWriter` allocates **154 KB per render instead of 254 KB**. Gen2 GCs over 6,300 renders went from **151 to 0**.
- **Test:** `RenderToWriterTests` checks that the output equals `Render(...).Html`, with and without beautify.
- **Not done:** the post-processor path still needs the string, because `IPostProcessor.PostProcessAsync` takes and returns a `string`.

### 15. Reduce the per-reader and per-element overhead of reading MJML
[Mjml.Net/Internal/HtmlReaderWrapper.cs](Mjml.Net/Internal/HtmlReaderWrapper.cs), [MjmlRenderContext.cs:54](Mjml.Net/MjmlRenderContext.cs#L54), [Component.cs](Mjml.Net/Component.cs)

**Potential: ~53 KB per reader, plus ~9 KB per render.**
- **Reader buffers:** each `HtmlPerformanceKit.HtmlReader` allocates about 53 KB of buffers up front, mostly two 10,240-char buffers. A render creates one, and every `mj-include` creates another one through `ReadFragment`. HtmlPerformanceKit (osjoberg/HtmlPerformanceKit) has no way to reuse a reader. A `Reset(TextReader)` method, or buffers taken from `ArrayPool<char>`, would make pooling possible. That change is upstream.
- **Small objects per element:** measured with allocation sampling.
  - `MjmlRenderContext.Read` assigns a new `OnError` closure on every call (`Action<HtmlError>` plus its display class, ~3.8 KB). The file name could be stored in a field instead.
  - `ReadSubtree()` allocates a `SubtreeReader` each time (~1.9 KB).
  - `IComponent.ChildNodes` is exposed as `IEnumerable<IComponent>`, so each `foreach` over it boxes the `List<T>` enumerator (~1.4 KB).
  - Together these are about 7–9 KB per render, roughly 3%.

**Partly done.**
- **✅ Error handler:** `Read` uses one `OnError` handler per context and keeps the current file in a field, restored after each call so includes still report their own file.
  - I first suspected that the `finally { OnError = null }` dropped errors after nested reads. It doesn't, because only the top-level wrapper subscribes to the parser's `ParseError` event.
- **✅ Child loops:** `Component.ChildNodes` now returns a `List<IComponent>`, and `IComponent.ChildNodes` is implemented explicitly as `IEnumerable<IComponent>`. `foreach` over it no longer boxes the enumerator. `Cleanup` uses it when the component is a `Component`.
- **Result:** both allocations are gone from the sample, and a render went from 261 to 256 KB. The output is byte-for-byte identical.
- **⏭️ `SubtreeReader`:** skipped. It is handed to custom components as the public `IHtmlReader`, so its lifetime is unknown and pooling it isn't safe.
- **⏭️ Reader buffers:** need a change in HtmlPerformanceKit (upstream).

---

## Rendering TODOs (phase-timed)

These five come from timing the phases of a plain `Render` (no post-processors) with `Stopwatch` counters in a temporary copy of the code, plus call counters. I didn't use the sampling profiler for these numbers: EventPipe samples at GC safe points and over-weights native calls and allocations (it showed `Guid.NewGuid` at 5%, which BenchmarkDotNet disproved).

Averages per render over the 21 templates (Release, .NET 10, ~50 components and ~244 tokens per template). Timings varied between runs from ~0.25 to ~0.40 ms per render, but the shares stayed stable:

| Phase | Beautify on | Beautify off |
|---|---|---|
| **Total** | **0.25–0.28 ms** | **~0.22 ms** |
| Read + build tree | ~0.10 ms (35%) | ~0.09 ms |
| – re-serializing text/raw content | ~0.024 ms | ~0.022 ms |
| – creating components | ~0.007 ms | ~0.006 ms |
| Bind | ~0.06 ms (22%) | ~0.055 ms |
| Measure | ~0.01 ms (3%) | – |
| Render | 0.065–0.075 ms (28%) | ~0.057 ms |
| `ToText` | ~0.031 ms (12%) | ~0.010 ms |

### 16. Bind only what is actually set, instead of querying every field
[Mjml.Net.Generator/Template.handlebar](Mjml.Net.Generator/Template.handlebar), [Mjml.Net/Internal/Binder.cs](Mjml.Net/Internal/Binder.cs)

**Potential: a large part of bind, which is ~22% of a render.**
- **The cost:** the generated `Bind()` calls `Binder.GetAttribute` for every `[Bind]` field, which is **1,166 calls per render for 50 components**.
  - Only 152 are found on the element itself and 5 through inheritance.
  - The other **1,008 (86%)** go through the class, parent-class, type and `mj-all` lookups, and almost all of them find nothing.
  - For templates with `mj-attributes`, each of those misses hashes two `AttributeKey` record structs, and `AttributeKey` uses randomized string hashing.
  - Bind comes to about 50 ns per field.
- **Idea:** invert it. Generate a `SetAttribute(string name, string value)` switch per component, and apply the sources that exist from lowest to highest precedence: `mj-all`, type, parent class, `mj-class`, inherited, own attributes. Each source only contains what is set. For inherited values, parents would expose the names they provide, since `GetInheritingAttribute` is a switch today.
- **Note on #4:** this corrects my "not a bottleneck" conclusion. That BenchmarkDotNet run had ±10–15% noise across the whole render, which isn't precise enough for a phase that is only 22% of it.

**✅ Done.**
- **`IBinder.Attributes`:** the binder resolves the effective attributes of an element once, on first access, and nothing else is involved. Its own attributes are already in the dictionary. The other sources are added with `TryAdd`, from the highest to the lowest precedence: inherited, own `mj-class` (last class first), parent `mj-class`, type, `mj-all`. `IBinder.GetAttribute` is gone.
- **Generated code:** `Bind()` loops over `Binder.Attributes` and switches on the name to set its own fields. Every class in the hierarchy does this for its own fields, so there are no callbacks between the binder and the component. The generated `GetAttribute(name)` falls back to `Binder.Attributes` for attributes the component has no field for.
- **Inherited values:** `IComponent.GetInheritingAttribute(name)` was replaced with `GetInheritingAttributes()`, which returns the name/value pairs a parent provides. Only 5 components implement it: accordion, accordion element, group, navbar and social.
- **Supporting changes:** `GlobalContext` indexes type attributes by element name. `IncludeComponent` reads `path` and `type` with the new `IHtmlReader.TryGetAttribute`, because it reads them before binding.
- **Result:** bind went from **~78 µs to ~21 µs per render (about −73%)**, measured with phase timers on both builds alternately. Render time was unchanged in the same runs, and allocations stayed at ~256 KB.
  - A first version, which applied the sources through callbacks into `Component`, reached ~14 µs. It was replaced by this simpler design: every class in the hierarchy now loops over the attributes, and the `mj-all` and type values are copied into each binder. The difference is ~7 µs (2–3% of a render).
- **Checks:**
  - Output is identical for all 21 templates, with and without beautify, using a fixed ID generator.
  - `AttributePrecedenceTests` covers each precedence step, parent classes and a shorthand combined with a side attribute from a class. I verified all cases against the old build before adding the tests.

### 17. Stop allocating tag names at every nesting level while reading
[Mjml.Net/Internal/HtmlReaderWrapper.cs:82](Mjml.Net/Internal/HtmlReaderWrapper.cs#L82)

**Potential: ~37 KB per render (~15% of allocations), plus CPU that grows with nesting depth.**
- **Chained readers:** `ReadSubtree()` wraps the current reader in another `SubtreeReader`, so a reader at depth *d* is a chain of *d* wrappers. Every token read goes through the whole chain.
- **A new string at every level:** each level calls `inner.Name` for its `VoidTags.Contains` check, and HtmlPerformanceKit creates a new string on every `Name` access. `ReadElement` and `ValidatingClosingState` read `Name` again.
- **Measured:** **777 name strings for 244 tokens per render**.
- **Idea:**
  - Keep one depth counter on the root `HtmlReaderWrapper`; each `SubtreeReader` only remembers its start depth, and `Read` returns `false` below it. Then every token is checked once instead of once per level.
  - Check void tags on `NameAsMemory.Span`, for example with a `switch` on length plus `SequenceEqual`, or a `HashSet` alternate lookup on .NET 9+.
  - Compare the closing tag in `ValidatingClosingState` as a span.

**✅ Done.**
- **One depth counter:** the root `HtmlReaderWrapper` tracks the depth of every token. It uses `NameAsMemory` and a `switch` on length for the void-tag check, so no string is allocated.
- **Flat subtrees:** a `SubtreeReader` only remembers its start depth and reads from the root. It ends when the depth drops below its start, either before reading, because a nested reader already consumed its end tag, or after reading.
- **Closing tags:** `ValidatingClosingState` compares the end tag with `NameAsSpan.SequenceEqual` and only allocates the name for the error message.
- **Result:** **257.5 KB → 232.4 KB per render (−10%)**. Time was roughly 10–15% faster in two rounds, but the timings on this machine are noisy.
- **Checks:**
  - Output is identical for all 21 templates, with and without beautify.
  - HTML and validation errors are identical to the old build for 11 malformed inputs: void tags in text and raw content, self-closing text, unclosed and wrongly nested tags, unknown elements, stray text and uppercase void tags.
  - An early version of this change failed one of those inputs. There, a self-closing `<mj-text />` was followed by a sibling section without whitespace between the tags, and the section got nested into the column, because the column's reader didn't notice that a nested reader had already read `</mj-column>`. `HtmlReaderTests.Should_end_parent_when_subtree_of_self_closing_element_reads_end_tag_of_parent` covers this case: it fails without the fix and passes with the old and the new implementation.

### 18. Intern tag and attribute names instead of allocating them per element
[Mjml.Net/MjmlRenderContext.cs:175](Mjml.Net/MjmlRenderContext.cs#L175), [Mjml.Net/MjmlRenderer.cs:100](Mjml.Net/MjmlRenderer.cs#L100)

**Potential: ~270 strings per render (~10 KB), and faster lookups.**
- **The cost:** each render allocates **169 attribute-name strings** (`GetAttributeName`) and about 100 tag-name strings for the component lookup, although both come from a small, fixed set: component names and `AllowedFields` keys.
- **Idea:** HtmlPerformanceKit already has `NameAsMemory` and `GetAttributeNameAsMemory(int)`.
  - Look up components by span, using `Dictionary.GetAlternateLookup<ReadOnlySpan<char>>` on .NET 9+ or a small span-keyed table otherwise.
  - Map attribute names to the interned strings of the component's `AllowedFields`, and only allocate unknown names.
  - Attribute values have to stay strings, because they are stored in fields.
- **Also:** creating a component (factory lookup plus `new T()`) takes ~7 µs per render (~3%). Explicit static factory lambdas instead of the generic `new()` constraint would make this cheaper.

**✅ Done (names), on .NET 9+.**
- **Tag names:** `MjmlRenderer.CreateComponent` looks up the component with `reader.NameAsSpan` through `Dictionary.GetAlternateLookup<ReadOnlySpan<char>>`. After that, `ReadElement` uses `component.ComponentName`, for example for the closing-tag check. It only allocates `reader.Name` for the "Invalid element" error.
- **Attribute names:** `BindComponentAttributes` looks up each name, via the new `IHtmlReader.GetAttributeNameAsSpan`, in the `AllowedFields` of the component. That is the dictionary the generated code already caches per type. The alternate lookup returns the existing key string. Names the component doesn't declare, such as `mj-class`, are still allocated. The renderer doesn't manage any names.
- **net7/net8:** `GetAlternateLookup` requires .NET 9, so these targets still allocate as before (`#if NET9_0_OR_GREATER`).
- **Result:** **232.4 KB → 223.7 KB per render (−3.7%)**. Output is identical for all 21 templates, and HTML and validation errors are identical for the malformed inputs from #17.
- **⏭️ Component factories:** not changed.

### 19. Reconsider `Beautify = true` as the default
[Mjml.Net/MjmlOptions.cs:50](Mjml.Net/MjmlOptions.cs#L50)

**Potential: 11–22% of render time and ~40% of output size, for everyone who doesn't need readable HTML.**
- **The cost:** beautify is on by default. It makes the output **49 KB instead of 30 KB** on average (+60%). That takes render time from ~0.22 ms to 0.25–0.28 ms, `ToText` from 0.010 ms to 0.031 ms, and it also increases what gets sent by email.
- **mjml's default:** as far as I know, mjml itself defaults to `beautify: false` (and deprecated the option in v4).
- **Idea:** change the default to `false` in a major version, or at least document the cost. Changing the default changes output, so it's a product decision. Callers can already pass `Beautify = false`.

### 20. Copy text and raw content instead of rebuilding it token by token
[Mjml.Net/Internal/HtmlReaderWrapper.cs:139](Mjml.Net/Internal/HtmlReaderWrapper.cs#L139)

**Potential: ~22–26 µs per render (~9%), plus many small strings.**
- **The cost:** for text components (`mj-text`, `mj-button`, `mj-raw` and others), `ReadInnerHtml` rebuilds the inner HTML from tokens. Every piece is added separately to an `InnerTextOrHtml`'s `List<string>`: `"<"`, the name, `" "`, the attribute name, `"="`, `"\""`, the value and `"\""`. Each `Name`, attribute name and value is a new string, and the list keeps growing from its default capacity of 10. The result is later appended to the output part by part.
- **Idea:**
  - Build the inner HTML directly into a pooled `StringBuilder` with the span APIs (`NameAsMemory`, `GetAttributeNameAsMemory`, `GetAttributeAsMemory`, `TextAsMemory`), and store a single string.
  - Better still: copy the original source text between the start and end tag. That needs character offsets, which HtmlPerformanceKit doesn't expose today, so it would be an upstream change.
- **Note:** the tag and attribute names are affected by #17 and #18 as well.

**✅ Done (raw content).**
- **One string instead of many parts:** `ReadInnerHtml` builds the inner HTML in a pooled `StringBuilder` with the span APIs of HtmlPerformanceKit, and returns an `InnerTextOrHtml` with a single string.
- **Trimming stays the same:** when the parts are written, whitespace-only parts at the end are dropped completely, including tabs, and then the last kept part is trimmed. That is not the same as trimming the concatenated string, which would keep for example a trailing `\n\t\t`. So `ReadInnerHtml` remembers where the last token that isn't whitespace-only text ended, keeping at least the first token, and cuts the string there. Trimming the start across parts is equivalent to trimming the concatenated string, so nothing extra is needed for that.
- **Not changed:** `ReadInnerText` (text-only components such as `mj-title` or `mj-navbar-link`) usually has a single text token already.
- **Result:** **223.7 KB → 218.4 KB per render (−2.4%)**.
- **Checks:**
  - Output is identical for all 21 templates.
  - HTML and validation errors are identical for the malformed inputs from #17.
  - HTML is identical to the old build for 96 content cases: `mj-text`, `mj-raw` and `mj-button`, with and without beautify. They cover empty and whitespace-only content, tabs and line breaks at the start and end, comments, attributes with spaces or empty values, a non-breaking space and nested tags.
  - `HtmlReaderTests.Should_read_inner_html_without_trailing_whitespace_text` fails with a naive version that doesn't cut the string, and `Should_read_empty_inner_html` checks empty content.

---

## Side findings (correctness, spotted during analysis)

- ✅ **`GlobalContext.Clear()` never clears `attributesByParentClass`** ([GlobalContext.cs:39](Mjml.Net/GlobalContext.cs#L39)). Render contexts are pooled, so parent-class attributes from one template carried over into later renders, and the dictionary kept growing. It is now cleared, and `GlobalContextTests` covers it.
- ✅ **`ColorType.Comparer.Equals` compared `rhs` with itself** ([Types/ColorType.cs:166](Mjml.Net/Types/ColorType.cs#L166)). It now compares `lhs` with `rhs`. In practice this almost never mattered: `HashSet` compares the full hash code first, and string hashes are randomized per process, so only a full 32-bit hash collision could be affected. That is also why there's no test for it.
- ✅ **`InnerTextOrHtml.AppendIntended` skipped the first character after each newline**, so after a blank line the next line was not indented (`"a\n\nb"`). Fixed, with a test in `InnerTextOrHtmlTests`. The benchmark templates render the same as before, because none of them has blank lines in indented content.
- ⏭️ **`SocialNetwork` calls `Defaults.ToList()` while iterating** ([SocialNetwork.cs:86](Mjml.Net/Components/Body/SocialNetwork.cs#L86)). This is fine: it runs once in the static constructor, and the copy is needed because the loop adds entries to the same dictionary.
- ✅ **`BreakpointComponent` wrote `context.Options.Breakpoint`.** That changed the caller's `MjmlOptions`, so an `mj-breakpoint` in one template carried over into later renders that reused the same options object. `GlobalContext.Breakpoint` now holds the breakpoint for the current render; it defaults to `Options.Breakpoint` and is reset in `Clear()`. `StyleHelper`, `ImageComponent` and `NavbarComponent` read it from there. `BreakpointTests` covers this: the test fails without the fix.
- ⚠️ **Not fixed: `mj-attributes` loses definitions in MJML without whitespace between tags** ([Components/Head/AttributesComponent.cs](Mjml.Net/Components/Head/AttributesComponent.cs)). After reading a tag's attributes, `AttributesComponent.Read` calls `htmlReader.Read()` again, which consumes the next token. Normally that token is the newline between two tags. In minified MJML it is the next definition, which then gets skipped. For example, `<mj-all color="red" /><mj-text color="blue" />` ignores the `mj-text` color, and the same happens for self-closing `<mj-class … />` definitions. Found while writing `AttributePrecedenceTests`; the behavior is the same before and after #16.
- ⚠️ **Not fixed: an unclosed text element at the end of the input throws** ([MjmlRenderContext.cs](Mjml.Net/MjmlRenderContext.cs), `ValidatingClosingState`). For example, `<mjml><mj-body><mj-section><mj-column><mj-text></mj-column></mj-section></mj-body></mjml>` throws `InvalidOperationException: Name property can only be accessed when TokenKind is Tag, EndTag or Doctype`. The inner text reader consumes everything up to the end of the input, and the closing check then reads `Name` without a valid token. The exception is thrown instead of a validation error being reported. It happens in the old and the new implementation of #17.
