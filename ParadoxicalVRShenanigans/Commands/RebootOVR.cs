using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class RebootOVRCommand : AsyncCommand
{
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context)
    {
        AnsiConsole.Clear();

        if(!Utils.IsElevated())
        {
            Logger.Log("failed to reboot ovr: not sudo");
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.RequiresElevation}[/]");
            return 1;
        }

        Logger.Log("rebooting OVR...");
        Utils.RebootOVR();

        return 0;
    }
}