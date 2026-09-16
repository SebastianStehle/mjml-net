namespace Mjml.Net.Compare;

public sealed record ComparisonRow(Measurement Net, Measurement Js)
{
    public double Speedup => Js.Median / Net.Median;
}

public sealed record Comparison(
    List<ComparisonRow> Rows,
    JsResults Js,
    MachineInfo Machine,
    DateTime Date,
    int Warmup,
    int Iterations)
{
    public double SpeedupGeoMean => Math.Exp(Rows.Average(x => Math.Log(x.Speedup)));

    public double SpeedupMin => Rows.Min(x => x.Speedup);

    public double SpeedupMax => Rows.Max(x => x.Speedup);

    public double TotalNetMs => Rows.Sum(x => x.Net.Median);

    public double TotalJsMs => Rows.Sum(x => x.Js.Median);

    public static Comparison Create(List<Measurement> netResults, JsResults jsResults, MachineInfo machine, int warmup, int iterations)
    {
        var jsByTemplate = jsResults.Measurements.ToDictionary(x => x.Template);

        var rows = netResults
            .Where(x => jsByTemplate.ContainsKey(x.Template))
            .Select(x => new ComparisonRow(x, jsByTemplate[x.Template]))
            .ToList();

        return new Comparison(rows, jsResults, machine, DateTime.UtcNow, warmup, iterations);
    }
}
