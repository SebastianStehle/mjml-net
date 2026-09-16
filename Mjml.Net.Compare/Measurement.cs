namespace Mjml.Net.Compare;

public sealed record Measurement(string Template, double FirstMs, double[] Timings, long AllocatedBytes = 0)
{
    public double Mean => Timings.Average();

    public double Median => Percentile(0.5);

    public double P95 => Percentile(0.95);

    private double Percentile(double p)
    {
        var sorted = Timings.Order().ToArray();
        var index = (int)Math.Clamp(Math.Ceiling(p * sorted.Length) - 1, 0, sorted.Length - 1);

        return sorted[index];
    }
}
