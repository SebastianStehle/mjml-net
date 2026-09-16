using System.Diagnostics;

namespace Mjml.Net.Compare;

public static class NetRunner
{
    private static readonly MjmlOptions Options = new MjmlOptions
    {
        Beautify = false
    };

    public static List<Measurement> Run(List<Template> templates, int warmup, int iterations)
    {
        var renderer = new MjmlRenderer();
        var watch = new Stopwatch();

        // Cold render of every template, before anything is warmed up.
        var firstRenders = new Dictionary<string, double>();

        foreach (var template in templates)
        {
            watch.Restart();
            renderer.Render(template.Source, Options);
            firstRenders[template.Name] = watch.Elapsed.TotalMilliseconds;
        }

        // Warm up across all templates, so that tiered JIT has optimized all code paths before measuring.
        for (var i = 0; i < warmup; i++)
        {
            foreach (var template in templates)
            {
                renderer.Render(template.Source, Options);
            }
        }

        var timings = templates.Select(_ => new double[iterations]).ToArray();
        var allocated = new long[templates.Count];

        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Round-robin over the templates, so that effects over time (JIT, CPU frequency) are distributed evenly.
        for (var i = 0; i < iterations; i++)
        {
            for (var t = 0; t < templates.Count; t++)
            {
                var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();

                watch.Restart();
                renderer.Render(templates[t].Source, Options);
                timings[t][i] = watch.Elapsed.TotalMilliseconds;

                allocated[t] += GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
            }
        }

        return templates
            .Select((template, t) => new Measurement(template.Name, firstRenders[template.Name], timings[t], allocated[t] / iterations))
            .ToList();
    }
}
