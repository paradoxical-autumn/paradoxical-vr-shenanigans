using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class DefaultCommand : AsyncCommand<DefaultCommand.Settings>
{
    public class Settings : CommandSettings
    {
        
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!AnsiConsole.Profile.Capabilities.Interactive)
        {
            AnsiConsole.MarkupLine($"[red]{Strings.Errors.NotInteractiveConsole}[/]");
            return 1;
        }

        while (true)
        {
            AnsiConsole.Clear();
            DisplayHeaderCommands();
            var choice = ShowMainMenu();

            try
            {
                switch (choice)
                {
                    case Strings.MenuOptions.DisableSteamVRHome:
                        AnsiConsole.Clear();

                        AnsiConsole.WriteLine();
                        await new SteamVRHomeKillCommand().ExecuteAsync(context, PromptForDSVRHSettings());
                        break;
                    case Strings.MenuOptions.InstallOculusKiller:
                        AnsiConsole.Clear();
                        AnsiConsole.WriteLine();
                        await new OCKCommand().ExecuteAsync(context, PromptForOCKSettings());
                        break;
                    case Strings.MenuOptions.Quit:
                        AnsiConsole.Clear();
                        return 0;

                }
            }
            catch (Exception ex)
            {
                Utils.PrintErr(ex);
            }

            if (!AnsiConsole.Confirm(Strings.Prompts.ReturnToMainMenu))
            {
                return 0;
            }
        }
    }

    private void DisplayHeaderCommands()
    {
        var table = new Table().HideHeaders().NoBorder();

        table.Title($"[yellow]{Strings.Application.AppName}[/]");
        table.AddColumn("col1", c => c.NoWrap().RightAligned().PadRight(3));
        table.AddColumn("col2", c => c.PadRight(0));
        table.AddEmptyRow();

        table.AddEmptyRow();
        table.AddRow(
            new Markup($"[yellow]{Strings.MenuOptions.DisableSteamVRHome}[/]"),
            new Markup(Strings.Descriptions.DisableSteamVRHome)
        );

        table.AddRow(
            new Markup($"[yellow]{Strings.MenuOptions.InstallOculusKiller}[/]"),
            new Markup(Strings.Descriptions.InstallOCK)
        );

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    private string ShowMainMenu()
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Strings.Prompts.InteractionPrompt)
                .HighlightStyle(new Style(foreground: Color.Orange1, decoration: Decoration.Bold))
                .EnableSearch()
                .AddChoices(new[] {
                            Strings.MenuOptions.DisableSteamVRHome,
                            Strings.MenuOptions.InstallOculusKiller,
                            Strings.MenuOptions.Quit
                }));
    }

    private SteamVRHomeKillCommand.Settings PromptForDSVRHSettings()
    {
        //bool prompt_path = AnsiConsole.Confirm(Strings.Prompts.HaveCustomisedSteamPath);

        string stm_pth = AnsiConsole.Prompt<string>(new TextPrompt<string>(Strings.Prompts.EnterSteamFolderPath).AllowEmpty());

        if (String.IsNullOrWhiteSpace(stm_pth))
            stm_pth = "C:\\Program Files (x86)\\Steam";

        var settings = new SteamVRHomeKillCommand.Settings
        {
            SteamPath = stm_pth
        };

        return settings;
    }

    private OCKCommand.Settings PromptForOCKSettings()
    {
        bool backup = AnsiConsole.Confirm(Strings.Prompts.BackupOculusDashExe);

        var set = new OCKCommand.Settings
        {
            BackupOCD = backup
        };

        return set;
    }
}