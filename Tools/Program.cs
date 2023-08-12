namespace Tools;

public static class Program
{
    public static void Main(string[] args)
    {
        MigrateCS.Run();

        ConvertJS.Run();
    }
}
