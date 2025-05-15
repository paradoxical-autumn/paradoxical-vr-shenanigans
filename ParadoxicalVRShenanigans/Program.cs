using ParadoxVrTools.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Text;
using Velopack;
using Velopack.Sources;

namespace ParadoxVrTools;
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        System.Console.InputEncoding = Encoding.UTF8;

        Logger.Log($"PVRS initialised! Running version {Utils.GetVersion()}...");
#if DEBUG
        Logger.Log("\n-----\nThis is a DEVELOPMENT build!\nAll hope abandon ye who enter here!\n-----");
#endif

        VelopackApp.Build().Run();
#if !DEBUG
        try
        {
            await UpdateApp(args);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error checking for update! -> {ex.ToString()}", true);
        }
#endif

        Logger.Log($"Initialising CommandApp");
        var app = new CommandApp<DefaultCommand>();
        app.Configure(config =>
        {
            config.SetApplicationName(Locale.Application.AppName);
            config.SetApplicationVersion(Utils.GetVersion());
        });

        return await app.RunAsync(args);
    }

    private static async Task UpdateApp(string[] args)
    {
        if (args.Contains("-s") || args.Contains("--skip-updates"))
        {
            Logger.Log("Skipping update check...");
            return;
        }
        
        Logger.Log("Checking for updates...");
        Console.WriteLine("Checking for updates...");
        
        var mgr = new UpdateManager(new GithubSource("https://github.com/paradoxical-autumn/paradoxical-vr-shenanigans", null, false));
        var newVersion = await mgr.CheckForUpdatesAsync();

        if (newVersion == null) return;

        if (!AnsiConsole.Confirm($"{Locale.Prompts.UpdateAvailable} ({Utils.GetVersion()} -> {newVersion.TargetFullRelease.Version})")) return;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync(Locale.Statuses.Starting, async ctx =>
            {
                await Task.Delay(1000);
                await mgr.DownloadUpdatesAsync(newVersion);
            });

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Aesthetic)
            .StartAsync(Locale.Statuses.Starting, async ctx =>
            {
                await Task.Delay(1000);
                mgr.ApplyUpdatesAndRestart(newVersion);
            });
    }
}
