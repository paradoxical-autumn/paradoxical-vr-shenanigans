using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class FixWhitebarCommand : AsyncCommand<FixWhitebarCommand.Settings>
{
    public class Settings : CommandSettings
    {

    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        bool stable = Utils.KillOVR();
        if (!stable)
        {
            Logger.Error("Failed to disable OVR. Bailing out.");
            return 1;
        }
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        startInfo.UseShellExecute = false;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C reg add \"HKEY_CURRENT_USER\\SOFTWARE\\Oculus\\RemoteHeadset\" /v \"numSlices\" /t REG_DWORD /d \"1\" /f";

        proc.StartInfo = startInfo;
        proc.Start();

        proc.WaitForExit();

        return 0;
    }
}