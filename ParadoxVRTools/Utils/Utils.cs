using Spectre.Console;
using System.Reflection;
using System.Security.Principal;

namespace ParadoxVrTools;
public static class Utils
{
    public readonly struct NewDirInfo
    {
        public NewDirInfo(DirectoryInfo di, string path)
        {
            DirInfo = di;
            Path = path;
        }

        public DirectoryInfo DirInfo { get; init; }
        public string Path { get; init; }

        public override string ToString() => $"{Path}";
    }

    public static NewDirInfo Create_Temp_Dir(long length = 8, long max_tries = 1000)
    {
        var tmp_dir = System.IO.Path.GetTempPath();
        Random rnd = new Random();
        var bytes = new byte[length];
        for (int i = 0; i <= max_tries; i++)
        {
            rnd.NextBytes(bytes);

            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = String.Concat(hexArray);

            if (Directory.Exists($"{tmp_dir}{hexStr}"))
                continue;

            DirectoryInfo di = Directory.CreateDirectory($"{tmp_dir}{hexStr}");

            return new NewDirInfo(di, $"{tmp_dir}{hexStr}");
        }
        throw new IOException("Could not create temporary directory.");
    }
    public static void PrintErr(object obj)
    {
        string s = (obj != null) ? obj.ToString()! : "NULL";
        try
        {
            AnsiConsole.MarkupLine("[red]{0}[/]", $"{string.Format(Strings.Errors.Exception, s)}".EscapeMarkup());
        }
        catch (Exception e)
        {
            AnsiConsole.WriteLine($"Exception happened while handling exception:\n{e}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Original exception follows:\n{s}");
        }
    }

    public static bool IsElevated()
    {
        if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
#pragma warning restore CA1416 // Validate platform compatibility
        }

        return false;
    }

    public static string GetVersion()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "NaN";

        return versionString;
    }
}