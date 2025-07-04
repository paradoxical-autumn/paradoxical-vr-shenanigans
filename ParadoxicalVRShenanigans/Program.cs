using ParadoxVrTools.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Text;
using Velopack;
using Velopack.Sources;
using System.Text.Json;

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
        try
        {
            await UpdateApp(args);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error checking for update! -> {ex.ToString()}", true);
        }

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
        string[] updateSkipArgs = {"-s", "--skip-updates", "-h", "--help"};
        if (args.Intersect(updateSkipArgs).Any())
        {
            Logger.Log("Skipping update check...");
            return;
        }
        
        string settingsFile = "./cfg/settings.json";
        DateTime now = DateTime.Now.ToUniversalTime();
        int unixTimestamp = (int)(now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        
        if (!Directory.Exists("cfg"))
        {
            Directory.CreateDirectory("cfg");
        }
        
        if (!File.Exists(settingsFile))
        {
            Logger.Log("Generating settings file...");
            ApplicationSettings settings = new ApplicationSettings
            {
                last_update_time = unixTimestamp,
            };
            string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFile, jsonString);
        }
        else
        {
            Logger.Log("Reading settings file...");
            string jsonString = File.ReadAllText(settingsFile);
            
            ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(jsonString)!;
            if (unixTimestamp - settings.last_update_time < 86400)
            {
                Logger.Log("Auto-skipping update check...");
                return;
            }
            else
            {
                Logger.Log("Updating settings file...");
                settings.last_update_time = unixTimestamp;
                jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(settingsFile, jsonString);
            }
        }
        
#if DEBUG
        return;
#endif
        
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
