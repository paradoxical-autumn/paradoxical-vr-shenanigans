using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class DefaultCommand : AsyncCommand<DefaultCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Prints helpful information about the app.")]
        [CommandOption("-i|--info|--information")]
        public bool PrintVersion { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!AnsiConsole.Profile.Capabilities.Interactive)
        {
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.NotInteractiveConsole}[/]");
            return 1;
        }

        if (settings.PrintVersion)
        {
            AnsiConsole.Write(new FigletText(Locale.Application.AppName));

            Panel pnl = Locale.NotStrings.SecretMenu();

            AnsiConsole.Write(pnl);

            var table = new Table();
            table.BorderColor(Color.SeaGreen3);


            table.AddColumn("METADATA").Centered();
            table.AddColumn("VALUE").Centered();

            table.AddRow("Version", Utils.GetVersion());
            table.AddRow("Compiled by", Locale.Meta.LastCompiler);

            table.LeftAligned();

            AnsiConsole.Write(table);

            pnl = new Panel(@"this is the cool stuff that makes this app work!!
https://spectreconsole.net/
https://github.com/BnuuySolutions/OculusKiller
C#
")
                .RoundedBorder()
                .Header("ATTRIBUTION");
            AnsiConsole.Write(pnl);
            return 0;
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
                    case Locale.MenuOptions.DisableSteamVRHome:
                        AnsiConsole.Clear();

                        AnsiConsole.WriteLine();
                        await new SteamVRHomeKillCommand().ExecuteAsync(context, PromptForDSVRHSettings());
                        break;
                    case Locale.MenuOptions.InstallOculusKiller:
                        AnsiConsole.Clear();
                        AnsiConsole.WriteLine();

                        var new_settings = PromptForOCKSettings();
                        if (new_settings == null)
                        {
                            break;
                        }

                        await new OCKCommand().ExecuteAsync(context, new_settings);
                        break;
                    case Locale.MenuOptions.Quit:
                        AnsiConsole.Clear();
                        return 0;

                }
            }
            catch (Exception ex)
            {
                Utils.PrintErr(ex);
            }

            if (!AnsiConsole.Confirm(Locale.Prompts.ReturnToMainMenu))
            {
                return 0;
            }
        }
    }

    private void DisplayHeaderCommands()
    {
        var table = new Table().HideHeaders().NoBorder();

        table.Title($"[royalblue1]{Locale.Application.AppName}[/] [grey]{Utils.GetVersion()}[/]");
        table.AddColumn("col1", c => c.NoWrap().RightAligned().PadRight(3));
        table.AddColumn("col2", c => c.PadRight(0));
        table.AddEmptyRow();

        table.AddEmptyRow();
        table.AddRow(
            new Markup($"[royalblue1]{Locale.MenuOptions.DisableSteamVRHome}[/]"),
            new Markup(Locale.Descriptions.DisableSteamVRHome)
        );

        table.AddRow(
            new Markup($"[royalblue1]{Locale.MenuOptions.InstallOculusKiller}[/]"),
            new Markup(Locale.Descriptions.InstallOCK)
        );

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            Panel pnl = Locale.NotStrings.WrongOS();
            AnsiConsole.Write(pnl);
        }
    }

    private string ShowMainMenu()
    {
        var prompt = new SelectionPrompt<string>()
                .Title(Locale.Prompts.InteractionPrompt)
                .HighlightStyle(new Style(foreground: Color.SeaGreen1, decoration: Decoration.Bold))
                .AddChoices(new[] {
                            Locale.MenuOptions.DisableSteamVRHome,
                            Locale.MenuOptions.InstallOculusKiller,
                            Locale.MenuOptions.Quit
                });

        prompt.HighlightStyle = new Style(Color.Black, Color.SeaGreen1 );
        prompt.SearchHighlightStyle = new Style(Color.Black, Color.MediumOrchid1 );
        prompt.WrapAround = true;

        return AnsiConsole.Prompt(prompt);
    }

    private SteamVRHomeKillCommand.Settings PromptForDSVRHSettings()
    {
        string stm_pth = AnsiConsole.Prompt<string>(new TextPrompt<string>(Locale.Prompts.EnterSteamFolderPath).AllowEmpty());

        if (String.IsNullOrWhiteSpace(stm_pth))
            stm_pth = "C:\\Program Files (x86)\\Steam";

        var settings = new SteamVRHomeKillCommand.Settings
        {
            SteamPath = stm_pth
        };

        return settings;
    }

    private OCKCommand.Settings? PromptForOCKSettings()
    {
        bool isAdmin = Utils.IsElevated();

        if (!isAdmin)
        {
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.RequiresElevation}[/]");
            return null;
        }

        bool backup = AnsiConsole.Confirm(Locale.Prompts.BackupOculusDashExe);
        string oc_pth = AnsiConsole.Prompt<string>(new TextPrompt<string>(Locale.Prompts.EnterOculusFolderPath).AllowEmpty());

        if (String.IsNullOrWhiteSpace(oc_pth))
            oc_pth = "C:\\Program Files\\Oculus";

        var set = new OCKCommand.Settings
        {
            BackupOCD = backup,
            OculusPath = oc_pth
        };

        return set;
    }
}