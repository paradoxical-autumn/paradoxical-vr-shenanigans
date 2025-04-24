using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ParadoxVrTools.Commands;

public class ElevateCommand
{
    public async Task<int> ExecuteAsync([NotNull] CommandContext context)
    {
        if (Utils.IsElevated())
        {
            Logger.Log("Not rebooting as an admin - already sudo");
            AnsiConsole.MarkupLine("You're already an administrator.");
            return 0;
        }

        var proc = new Process
        {
            StartInfo =
            {
                FileName = Environment.ProcessPath,
                UseShellExecute = true,
                Verb = "runas"
            }
        };
        
        AnsiConsole.MarkupLine("Rebooting as administrator... this window will now close.");
        
        proc.Start();
        
        Logger.ImproperExit("rebooting as admin.", false, 0);
        
        // no return.

        return 0;
    }
}