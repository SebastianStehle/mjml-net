namespace ConsoleApp22.Components
{
    public sealed class HeadContext
    {
        public List<(string Name, string Link)> Fonts { get; } = new List<(string Name, string Link)>();

        public List<(string Style, bool Inline)> Styles { get; } = new List<(string Style, bool Inline)>();

        public string? Breakpoint { get; set; }
    }
}
