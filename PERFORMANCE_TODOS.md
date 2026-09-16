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

### 6. Avoid string allocations in the hot rendering helpers
- `WriterExtensions.StyleIfNumber`, `StyleIf`, and `AttrOrAuto` ([Extensions/WriterExtensions.cs](Mjml.Net/Extensions/WriterExtensions.cs)) use `$"{value}{unit}"`, which builds a string only to append it. Route these through the existing interpolated-handler overloads, such as `Style(string, ref StyleInterpolatedStringHandler)`, so they write straight into the buffer.
- `ColumnComponent.Measure` builds `FormattableString.Invariant($"{widthValue}%")` for every column. `MediaQuery.Width` does the same for each media query.
- `ColorType.Coerce` allocates a `char[]` and then a `string`. Use `string.Create`.
- `Style.Static` allocates a closure for each style. Make it a subclass or a static lambda with state.

### 7. Parse shorthand values without `Split`
[Mjml.Net/BindingHelper.cs:34](Mjml.Net/BindingHelper.cs:34), [SectionComponent.cs:357,494](Mjml.Net/Components/Body/SectionComponent.cs:357), [MsoButtonComponent.cs:46](Mjml.Net/Components/Body/MsoButtonComponent.cs:46), [Binder.ClassNames](Mjml.Net/Internal/Binder.cs:15)

`ParseShorthandValue` runs for every `padding` or `border-radius` shorthand in generated `Bind()`. It allocates a `string[]` and substrings. For the common 1-part case (`padding="10px"`), return the original string with no allocation. For 2–4 parts, scan with `ReadOnlySpan<char>.IndexOf(' ')`. The `mj-class` split and the background-position split can use the same approach.

### 8. Parse each unit value once, not in every `Measure`/`Render`
[ColumnComponent.cs:120](Mjml.Net/Components/Body/ColumnComponent.cs:120), [SectionComponent.cs:94](Mjml.Net/Components/Body/SectionComponent.cs:94), [ImageComponent.cs:109](Mjml.Net/Components/Body/ImageComponent.cs:109), [HeroComponent.cs](Mjml.Net/Components/Body/HeroComponent.cs), [ButtonComponent.cs:197](Mjml.Net/Components/Body/ButtonComponent.cs:197)

`UnitParser.Parse` runs 4–6 times per component, and often on `null`. It is cheap, but it runs very often. Three improvements:
- Return early on `null` before `IsNullOrWhiteSpace` and `Trim`.
- Parse `px` integers by hand instead of calling `int.TryParse` with `NumberStyles.Any` and a culture.
- Cache parsed paddings and borders on the component in `Bind`.

Also, `char.IsNumber` accepts Unicode digits. `char.IsAsciiDigit` is faster and correct here.

### 9. Reduce per-render overhead in the render pipeline
- `RenderCore` creates a `new MjmlOptions()` record on every call when `options` is null. Use a static default instance. `MjmlRenderContext.Setup` does the same thing again.
- `Render(TextReader)` and `RenderAsync(Stream)` read the whole input into a `string`, and then `HtmlReaderWrapper` wraps it again in a `StringReader`. Pass the `TextReader` straight to `HtmlReader`.
- `StartBuffer` allocates a `new RenderBuffer` class for each buffer. Nested buffers are used for conditional or MSO sections, so pool these as well.
- `BindComponentAttributes` creates a `SourcePosition` for every attribute even when `Validator` is null. Guard it with `if (mjmlOptions.Validator != null)`.
- `ReadElement` looks up the component factory in a `Dictionary<string, Func<IComponent>>` and then calls a delegate. `new T()` in a generic lambda goes through `Activator`. Use a generated `switch` or a `FrozenDictionary` of static factory lambdas.
- `RenderBuffer.WriteLineStart` and `InnerTextOrHtml.WriteLineStart` append spaces in a loop. Use `sb.Append(' ', indent * 2)`.
- `Validate()` copies the error list even when it is empty.

### 10. Make the AngleSharp post-processing path cheaper
[Mjml.Net.PostProcessors/InlineCssPostProcessor.cs](Mjml.Net.PostProcessors/InlineCssPostProcessor.cs), [AngleSharpPostProcessor.cs](Mjml.Net.PostProcessors/AngleSharpPostProcessor.cs)

When post-processors are on, this is probably the most expensive part of rendering, though I have not measured it. The code already reuses the parsed document across inner processors, but:
- `ProcessAsync` walks the full DOM **three times**, and `Traverse` allocates a `ToList()` copy of the children at every node.
- `InlineStyle` calls `document.Context.GetService<IRenderDevice>()` and `view.GetStyleCollection(device)` **for every element**. Build the style collection once per document.
- `RenameTag` clones `<style>` elements by serializing and re-parsing `InnerHtml`. Instead, record the non-inline style elements, disable them temporarily (for example by moving them out of the tree), and put them back afterwards.
- `BrowsingContext.New` runs for every render. Check whether a context can be reused.
- Add a benchmark that runs `RenderAsync` with `AngleSharpPostProcessor.Default`. The current benchmark only covers the sync path.

---

## Side findings (correctness, spotted during analysis)

- **`GlobalContext.Clear()` never clears `attributesByParentClass`** ([GlobalContext.cs:34](Mjml.Net/GlobalContext.cs:34)). Render contexts are pooled, so parent-class attributes from one template carry over into later renders. The dictionary also keeps growing, which is a memory leak.
- **`ColorType.Comparer.Equals` compares `rhs` with itself** ([Types/ColorType.cs:166](Mjml.Net/Types/ColorType.cs:166)). As a result, any value whose hash collides with a named color is treated as valid.
- `SocialNetwork` calls `Defaults.ToList()` while iterating ([SocialNetwork.cs:86](Mjml.Net/Components/Body/SocialNetwork.cs:86)). Check whether this code runs per render or only once.
