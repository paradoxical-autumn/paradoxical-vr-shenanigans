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

        Logger.Log($"PVRS initialised! Running version {Utils.GetVersion()}... person to blame: {Locale.Meta.LastCompiler}");

        VelopackApp.Build().Run();
#if !DEBUG
        Logger.Log("Checking for updates...");
        try
        {
            await UpdateApp();
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
        });

        return await app.RunAsync(args);
    }

    private static async Task UpdateApp()
    {
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
