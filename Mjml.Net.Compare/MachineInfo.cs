using System.Runtime.InteropServices;

namespace Mjml.Net.Compare;

public sealed record MachineInfo(string Os, string Cpu, int Cores, string DotNetVersion, string MjmlNetVersion)
{
    public static MachineInfo Detect()
    {
        return new MachineInfo(
            $"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})",
            DetectCpu(),
            Environment.ProcessorCount,
            Environment.Version.ToString(),
            typeof(MjmlRenderer).Assembly.GetName().Version?.ToString(3) ?? "dev");
    }

    private static string DetectCpu()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");

                if (key?.GetValue("ProcessorNameString") is string name)
                {
                    return name.Trim();
                }
            }
            else if (File.Exists("/proc/cpuinfo"))
            {
                var line = File.ReadLines("/proc/cpuinfo").FirstOrDefault(x => x.StartsWith("model name", StringComparison.Ordinal));

                if (line != null)
                {
                    return line[(line.IndexOf(':') + 1)..].Trim();
                }
            }
        }
        catch
        {
            // Fall back to the generic description.
        }

        return RuntimeInformation.ProcessArchitecture.ToString();
    }
}
