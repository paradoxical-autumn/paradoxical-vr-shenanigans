using Spectre.Console;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;

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
            Logger.Error(s, true);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception happened while handling exception:\n{e}");
            AnsiConsole.WriteLine();
            Logger.Error($"Original exception follows:\n{s}");
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

        Logger.Log("Unable to get administrator permissions as we're not running windows.");

        return false;
    }

    public static string GetVersion()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        var versionString = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "NaN";

        return versionString;
    }
    
    public static void RebootOVR()
    {
        AnsiConsole.Write("rebooting OVR, you may notice a screen flash");
        Logger.Log("aight, we're rebooting OVR. you know what that means... fish!");
        Logger.Error("Rebooting OVR via RebootOVR() is deprecated. Please use KillOVR() instead.", true, true);
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        startInfo.UseShellExecute = false;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C net stop OVRService";

        proc.StartInfo = startInfo;
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Grenade)
            .Start("Stopping OVR...\n\n", ctx =>
            {
                proc.Start();

                proc.WaitForExit();
            });
        

        startInfo.Arguments = "/C net start OVRService";
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Clock)
            .Start("Rebooting OVR...\n\n", ctx =>
            {
                proc.Start();

                proc.WaitForExit();
            });
    }

#pragma warning disable CA1416 // Validate platform compatibility
    public static bool KillOVR()
    {
        if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            return false;
        }
        ServiceController sc = new ServiceController("OVRService");

        if (sc.Status == ServiceControllerStatus.Stopped)
        {
            Logger.Log("OVRService has already stopped. Not rebooting.");
            return true;
        }
        
        AnsiConsole.Write("Stopping all OVR related processes, you may notice a screen flash");
        Logger.Log("Killing OVR now...");

        if (!sc.CanStop)
        {
            Logger.Error("Unable to stop OVRService!! Bailing out...");
            return false;
        }

        try
        {
            AnsiConsole.Status()
                .Spinner(Spinner.Known.Grenade)
                .Start("Stopping OVR...\n\n", ctx => { sc.Stop(); });
        }
        catch (InvalidOperationException ex)
        {
            Logger.Error($"Unable to stop OVRService: {ex.ToString()}");
            return false;
        }
        
        
        return true;
    }
#pragma warning restore CA1416 // Validate platform compatibility
}