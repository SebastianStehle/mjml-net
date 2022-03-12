namespace Mjml.Net.Helpers
{
    public sealed record Breakpoint(string Value)
    {
        public static readonly Breakpoint Default = new Breakpoint("480px");
    }
}
