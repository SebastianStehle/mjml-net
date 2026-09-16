# Performance Analysis – Mjml.Net

I found these by reading the code. I did **not** run benchmarks to measure them. To check the effect of any change, run `Mjml.Net.Benchmark` (`TemplateBenchmarks`, 21 templates, `[MemoryDiagnoser]`) before and after the change.

## How rendering works (hot path)

`MjmlRenderer.RenderCore` → `MjmlRenderContext.Read` (HtmlPerformanceKit tokenizer wrapped by `HtmlReaderWrapper`/`SubtreeReader`) → for each tag: `CreateComponent` + pooled `Binder` → `Bind` (generated code: one `Binder.GetAttribute` + `Coerce` per `[Bind]` field) → `Measure` → `Render` into a pooled `StringBuilder` (`RenderBuffer`) → helpers (`StyleHelper`, `FontHelper`, …) → `ToText()`. The async path can also run AngleSharp post-processors, which parse the whole document again.

Allocation matters most here. On a typical template, most of the cost is attribute resolution during bind, string building during render, and short-lived objects created for every component.

---

## TODOs

### 1. ✅ Fix O(n²) sibling counting in `Component.MeasureChildren`
[Mjml.Net/Component.cs:131](Mjml.Net/Component.cs:131)

```csharp
child.Measure(context, width, childNodes.Count, childNodes.Count(x => !x.Raw));
```

For every child, this runs a LINQ `Count` over all siblings again. That is O(n²) per parent, and it allocates an enumerator and a closure every time. Count the non-raw children once before the loop. This path runs for every container: body, section, column, group, hero, and wrapper.

### 2. ✅ Keep large `StringBuilder`s in the pool
[Mjml.Net/DefaultPools.cs:8](Mjml.Net/DefaultPools.cs:8)

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
[Mjml.Net/Internal/Binder.cs:60](Mjml.Net/Internal/Binder.cs:60)

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
[Mjml.Net/GlobalContext.cs](Mjml.Net/GlobalContext.cs), [Helpers/Style.cs](Mjml.Net/Helpers/Style.cs), [Components/Body/BodyComponent.cs:39-47](Mjml.Net/Components/Body/BodyComponent.cs:39)

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
[Mjml.Net/BindingHelper.cs:34](Mjml.Net/BindingHelper.cs:34), [SectionComponent.cs:357,494](Mjml.Net/Components/Body/SectionComponent.cs:357), [MsoButtonComponent.cs:46](Mjml.Net/Components/Body/MsoButtonComponent.cs:46), [Binder.ClassNames](Mjml.Net/Internal/Binder.cs:15)

`ParseShorthandValue` runs for every `padding` or `border-radius` shorthand in generated `Bind()`. It allocates a `string[]` and substrings. For the common 1-part case (`padding="10px"`), return the original string with no allocation. For 2–4 parts, scan with `ReadOnlySpan<char>.IndexOf(' ')`. The `mj-class` split and the background-position split can use the same approach.

**⏭️ Measured, not worth it.** Across all `Split` calls in the library, `string[]` allocations add up to about 5.6 KB per render, about 2% of the total. The substrings would still be allocated, because they are stored in fields. `ParseShorthandValue("10px 25px")` takes 38 ns and allocates 104 B. For one value, `Split` already returns the original string without a substring, so the only saving would be the array.

### 8. Parse each unit value once, not in every `Measure`/`Render`
[ColumnComponent.cs:120](Mjml.Net/Components/Body/ColumnComponent.cs:120), [SectionComponent.cs:94](Mjml.Net/Components/Body/SectionComponent.cs:94), [ImageComponent.cs:109](Mjml.Net/Components/Body/ImageComponent.cs:109), [HeroComponent.cs](Mjml.Net/Components/Body/HeroComponent.cs), [ButtonComponent.cs:197](Mjml.Net/Components/Body/ButtonComponent.cs:197)

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

### 12. Stop AngleSharp from re-parsing the CSS we just wrote
[Mjml.Net.PostProcessors/InlineCssPostProcessor.cs](Mjml.Net.PostProcessors/InlineCssPostProcessor.cs)

**Potential: ~7.3 ms and ~3.7 MB per render, about 25% of post-processing.**
- **The cost:** `element.SetAttribute("style", css)` triggers AngleSharp.Css's `StyleAttributeObserver`, which parses the CSS string again into the element's style declaration. The declaration is thrown away, because we only serialize afterwards. Timed separately, the `SetAttribute` calls alone take 7.3 ms and allocate 3.7 MB.
- **Idea:** compute all declarations first. This is equivalent, because inlining already runs children before parents and doesn't read computed children. Keep the CSS strings in a `Dictionary<IElement, string>`, and emit them from a custom `IMarkupFormatter` during `ToHtml` instead of calling `SetAttribute`.

### 13. Reuse the AngleSharp `BrowsingContext`
[Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs:54](Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs:54)

**Potential: ~1 ms and ~387 KB per render.**
- **The cost:** `BrowsingContext.New(HtmlConfiguration)` runs on every render. Creating a context alone takes about 1 ms and allocates 387 KB (services, factories, entity provider).
- **Idea:** a context can open many documents, but it isn't thread-safe. Pool contexts with an `ObjectPool<IBrowsingContext>`, and dispose or close each document after `ToHtml`.
- **Check:** confirm that nothing accumulates in a reused context, such as history or the active document.

### 14. Don't create the full HTML string when the caller doesn't need it
[Mjml.Net/MjmlRenderer.cs:181](Mjml.Net/MjmlRenderer.cs:181), [RenderBuffer.cs](Mjml.Net/RenderBuffer.cs)

**Potential: ~100 KB per render (~39% of what remains), and most gen2 GCs.**
- **Every GC is gen2:** over 6,300 renders, gen0/gen1/gen2 collections were 161/161/161. 57% of the returned HTML strings are at least 85 KB, so they are allocated on the large-object heap, and those allocations are what trigger the full collections. On a server with a large heap, gen2 GCs are the expensive kind.
- **The result string is the largest remaining allocation**, at about 100 KB per render.
- **Idea:** add overloads such as `Render(string mjml, TextWriter output, MjmlOptions?)` and `RenderAsync(..., Stream output, ...)`. They would write the `StringBuilder` chunks (`GetChunks()`) directly, for example into an ASP.NET response or an email library.
- **With post-processors:** the HTML is materialized as a string first and then parsed. Passing AngleSharp the buffer content instead would avoid one full copy.

### 15. Reduce the per-reader and per-element overhead of reading MJML
[Mjml.Net/Internal/HtmlReaderWrapper.cs](Mjml.Net/Internal/HtmlReaderWrapper.cs), [MjmlRenderContext.cs:51](Mjml.Net/MjmlRenderContext.cs:51), [Component.cs](Mjml.Net/Component.cs)

**Potential: ~53 KB per reader, plus ~9 KB per render.**
- **Reader buffers:** each `HtmlPerformanceKit.HtmlReader` allocates about 53 KB of buffers up front, mostly two 10,240-char buffers. A render creates one, and every `mj-include` creates another one through `ReadFragment`. HtmlPerformanceKit (osjoberg/HtmlPerformanceKit) has no way to reuse a reader. A `Reset(TextReader)` method, or buffers taken from `ArrayPool<char>`, would make pooling possible. That change is upstream.
- **Small objects per element:** measured with allocation sampling.
  - `MjmlRenderContext.Read` assigns a new `OnError` closure on every call (`Action<HtmlError>` plus its display class, ~3.8 KB). The file name could be stored in a field instead.
  - `ReadSubtree()` allocates a `SubtreeReader` each time (~1.9 KB).
  - `IComponent.ChildNodes` is exposed as `IEnumerable<IComponent>`, so each `foreach` over it boxes the `List<T>` enumerator (~1.4 KB).
  - Together these are about 7–9 KB per render, roughly 3%.

---

## Side findings (correctness, spotted during analysis)

- ✅ **`GlobalContext.Clear()` never clears `attributesByParentClass`** ([GlobalContext.cs:34](Mjml.Net/GlobalContext.cs:34)). Render contexts are pooled, so parent-class attributes from one template carried over into later renders, and the dictionary kept growing. It is now cleared, and `GlobalContextTests` covers it.
- ✅ **`ColorType.Comparer.Equals` compared `rhs` with itself** ([Types/ColorType.cs:166](Mjml.Net/Types/ColorType.cs:166)). It now compares `lhs` with `rhs`. In practice this almost never mattered: `HashSet` compares the full hash code first, and string hashes are randomized per process, so only a full 32-bit hash collision could be affected. That is also why there's no test for it.
- ✅ **`InnerTextOrHtml.AppendIntended` skipped the first character after each newline**, so after a blank line the next line was not indented (`"a\n\nb"`). Fixed, with a test in `InnerTextOrHtmlTests`. The benchmark templates render the same as before, because none of them has blank lines in indented content.
- ⏭️ **`SocialNetwork` calls `Defaults.ToList()` while iterating** ([SocialNetwork.cs:86](Mjml.Net/Components/Body/SocialNetwork.cs:86)). This is fine: it runs once in the static constructor, and the copy is needed because the loop adds entries to the same dictionary.
- ⚠️ **Not fixed: `BreakpointComponent` writes `context.Options.Breakpoint`.** That changes the `MjmlOptions` instance the caller passed in, so an `mj-breakpoint` in one template carries over into later renders that reuse the same options object. Fixing it means storing the breakpoint per render instead of on the options. That changes where `StyleHelper` and others read it, so it needs a decision on the public behavior.
