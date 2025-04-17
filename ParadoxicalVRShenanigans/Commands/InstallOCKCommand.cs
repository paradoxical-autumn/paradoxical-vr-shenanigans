using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class OCKCommand : AsyncCommand<OCKCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description(Locale.Descriptions.BackupOCD)]
        [CommandOption("-b|--backup")]
        public bool BackupOCD { get; set; }

        [Description(Locale.Descriptions.OculusPath)]
        [CommandOption("-p|--path")]
        public string? OculusPath { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        AnsiConsole.Clear();

        bool isAdmin = Utils.IsElevated();

        if (!isAdmin)
        {
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.RequiresElevation}[/]");
            return 1;
        }

        string path_to_dash_folder = $"{settings.OculusPath}/Support/oculus-dash/dash/bin";
        FileAttributes attr;

        try
        {
            // verify paths exist.
            attr = File.GetAttributes(path_to_dash_folder);
            attr = File.GetAttributes($"{settings.OculusPath}/Support/oculus-dash/dash/bin");
            attr = File.GetAttributes($"{settings.OculusPath}/Support/oculus-dash/dash/bin/OculusDash.exe");
        }
        catch (Exception ex)
        {
            Utils.PrintErr(ex);
            Logger.Error($"{ex}");
            return 1;
        }

        Utils.NewDirInfo di = Utils.Create_Temp_Dir();

        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn()
                {
                    CompletedStyle = new Style(Color.SeaGreen1),
                    RemainingStyle = new Style(Color.Grey15),
                    FinishedStyle = new Style(Color.SeaGreen1),
                },
                new PercentageColumn()
                {
                    CompletedStyle = new Style(Color.SeaGreen1)
                },
                new RemainingTimeColumn(),
                new SpinnerColumn(Spinner.Known.Point),
            })
            .StartAsync(async ctx =>
            {
                Logger.Log("Downloading O.K.");
                var task1 = ctx.AddTask("Downloading OculusKiller");

                task1.Value = 1;

                HttpClient client = new HttpClient();
                var resp = await client.GetAsync("https://github.com/BnuuySolutions/OculusKiller/releases/latest/download/OculusDash.exe", HttpCompletionOption.ResponseHeadersRead);

                var totalBytes = resp.Content.Headers.ContentLength ?? -1L;
                var canReportProgress = totalBytes != -1;
                var totalBytesRead = 0L;
                var readChunkSize = 8192;

                using (var cs = await resp.Content.ReadAsStreamAsync())
                using (var fs = new FileStream($"{di.Path}/OculusDash.exe", FileMode.Create, FileAccess.Write, FileShare.None, readChunkSize, true))
                {
                    var buffer = new byte[readChunkSize];
                    int bytesRead;

                    while ((bytesRead = await cs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fs.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += bytesRead;

                        if (canReportProgress)
                        {
                            task1.Value = Math.Round((double)totalBytesRead / totalBytes * 100, 2);
                        }
                    }
                }
            });

        Logger.Log("finished downloading O.K.");

        if (settings.BackupOCD)
        {
            TimeSpan epoch = DateTime.Now - new DateTime(1970, 1, 1);
            Logger.Log($"backing up oldé oculus runtime to: {path_to_dash_folder}/OculusDash.exe.{(int)epoch.TotalSeconds}.bak");
            await AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBar)
                .StartAsync("[italic royalblue1]backing up oculus...[/]", async ctx =>
                {
                    MoveFile($"{path_to_dash_folder}/OculusDash.exe", $"{path_to_dash_folder}/OculusDash.exe.{(int)epoch.TotalSeconds}.bak");
                    //Thread.Sleep(1000);
                });

            AnsiConsole.MarkupLine($"[italic Seagreen1]backed up! find it at '{path_to_dash_folder}/OculusDash.exe.{(int)epoch.TotalSeconds}.bak'[/]");
        }

        Logger.Log("installing oculus dash replacement");
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .StartAsync("[italic royalblue1]installing new OculusDash.exe[/]", async ctx =>
            {
                MoveFile($"{di.Path}/OculusDash.exe", $"{path_to_dash_folder}/OculusDash.exe");
                //Thread.Sleep(1000);
            });

        AnsiConsole.MarkupLine("[italic seagreen1]installed![/]");

        AnsiConsole.WriteLine();

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Grenade)
            .Start("[italic lightslateblue]cleaning up[/]", ctx =>
            {
                // clean up.
                di.DirInfo.Delete(true);

                // lets fake some delay :3 // NO LET'S NOT.
                //Thread.Sleep(1000);
            });

        AnsiConsole.MarkupLine("[italic seagreen1]Finished all tasks successfully.[/]");

        if (!AnsiConsole.Confirm(Locale.Prompts.RebootOVR))
        {
            AnsiConsole.MarkupLine("Okay, you'll have to reboot your PC to finish the installation.");
            return 0;
        }

        AnsiConsole.MarkupLine("Got it, rebooting OVR.");

        Utils.RebootOVR();

        return 0;
    }

    private static async void MoveFile(string sourceFile, string destinationFile)
    {
        using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
        {
            using (FileStream destinationStream = File.Create(destinationFile))
            {
                await sourceStream.CopyToAsync(destinationStream);
                sourceStream.Close();
                destinationStream.Close();
            }
        }
    }
}