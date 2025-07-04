using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ParadoxVrTools.Commands;

public class DebugMenu
{
    public async Task<int> ExecuteAsync([NotNull] CommandContext context)
    {
        var choice = ShowDebugMenu();
        
        switch (choice)
        {
            case Locale.MenuOptions.OpenLogs:
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = "explorer.exe",
                        ArgumentList = { $"{Logger.LogsFolder}" },
                        UseShellExecute = true,
                    }
                };
                proc.Start();
                break;
            
            case Locale.MenuOptions.CheckForUpdates:
                await Utils.CheckForUpdates();
                break;
            
            case Locale.MenuOptions.Quit:
                return 0;
                break;
        }

        return 0;
    }

    private string ShowDebugMenu()
    {
        var prompt = new SelectionPrompt<string>()
            .Title($"Debug Menu - {Locale.Prompts.InteractionPrompt}")
            .HighlightStyle(new Style(foreground: Color.SeaGreen1, decoration: Decoration.Bold))
            .AddChoices(new[] {
                Locale.MenuOptions.OpenLogs,
                Locale.MenuOptions.CheckForUpdates,
                Locale.MenuOptions.Quit
            });
        
        prompt.HighlightStyle = new Style(Color.Black, Color.SeaGreen1 );
        prompt.SearchHighlightStyle = new Style(Color.Black, Color.MediumOrchid1 );
        prompt.WrapAround = true;

        return AnsiConsole.Prompt(prompt);
    }
    
    
}