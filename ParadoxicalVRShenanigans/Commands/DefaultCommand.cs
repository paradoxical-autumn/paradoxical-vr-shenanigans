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
        
        // This goes unused here because it is checked before we reach this point.
        // It simply exists here for the help command to pick up on.
        [Description("Skips checking for updates")]
        [CommandOption("-s|--skip-updates")]
        public bool _ { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!AnsiConsole.Profile.Capabilities.Interactive)
        {
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.NotInteractiveConsole}[/]");
            Logger.Error("Not interactive console. Exiting...");
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
https://velopack.io/
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
            Logger.Log("Creating menu.");
            DisplayHeaderCommands();
            var choice = ShowMainMenu();

            Logger.Log($"Attempting to run choice: {choice}");

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
                    case Locale.MenuOptions.FixOculusWhiteBar:
                        AnsiConsole.Clear();
                        AnsiConsole.WriteLine();

                        if (!Utils.IsElevated())
                        {
                            AnsiConsole.MarkupLine($"[red]{Locale.Errors.RequiresElevation}[/]");
                            break;
                        }

                        await new FixWhitebarCommand().ExecuteAsync(context, new FixWhitebarCommand.Settings());

                        break;
                    case Locale.MenuOptions.RebootOVR:
                        AnsiConsole.Clear();

                        await new RebootOVRCommand().ExecuteAsync(context);

                        break;
                    case Locale.MenuOptions.ElevateApp:
                        AnsiConsole.Clear();

                        await new ElevateCommand().ExecuteAsync(context);

                        break;
                        
                    case Locale.MenuOptions.DebugOptions:
                        AnsiConsole.Clear();
                        
                        await new DebugMenu().ExecuteAsync(context);

                        break;
                    case Locale.MenuOptions.Quit:
                        AnsiConsole.Clear();
                        return 0;

                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[red]\n---[[CRITICAL ERROR]]---\n:(\nAn unhandled exception occurred. Please report it to the developers![/]");
                AnsiConsole.WriteException(ex, new ExceptionSettings
                {
                    Format = ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks,
                    Style = new ExceptionStyle
                    {
                        Exception = new Style().Foreground(Color.Grey),
                        Message = new Style().Foreground(Color.White),
                        NonEmphasized = new Style().Foreground(Color.Cornsilk1),
                        Parenthesis = new Style().Foreground(Color.Cornsilk1),
                        Method = new Style().Foreground(Color.Red),
                        ParameterName = new Style().Foreground(Color.Cornsilk1),
                        ParameterType = new Style().Foreground(Color.Red),
                        Path = new Style().Foreground(Color.Red),
                        LineNumber = new Style().Foreground(Color.Cornsilk1),
                    }
                });
                AnsiConsole.MarkupLine($"\n[red]Please grab the current log file and send it our way. Thanks!![/]\n-> [royalblue1]Find it here:[/] {Logger.LogLocation}\n\nWhile you can continue using the app, it is not recommended to do so.\n");
                Logger.Error($"Unhandled exception\n->{ex.ToString()}", true, true);
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

        table.AddRow(
            new Markup($"[royalblue1]{Locale.MenuOptions.FixOculusWhiteBar}[/]"),
            new Markup(Locale.Descriptions.FixOculusWhiteBar.EscapeMarkup())
        );

        table.AddRow(
            new Markup($"[royalblue1]{Locale.MenuOptions.RebootOVR}[/]"),
            new Markup(Locale.Descriptions.RebootOVR.EscapeMarkup())
        );

        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            Logger.Log($"""
----------
DO NOT REPORT ERRORS ON THIS BUILD. IT IS RUNNING AN UNSUPPORTED OPERATING SYSTEM.
IT IS RUNNING {System.Environment.OSVersion.Platform}
----------
""");
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
                            Locale.MenuOptions.FixOculusWhiteBar,
                            Locale.MenuOptions.RebootOVR,
                            Locale.MenuOptions.ElevateApp,
                            Locale.MenuOptions.DebugOptions,
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
            Logger.Log("Unable to proceed: not sudo.");
            AnsiConsole.MarkupLine($"[red]{Locale.Errors.RequiresElevation}[/]");
            return null;
        }

        Logger.Log("Prompting to for backup");
        bool backup = AnsiConsole.Confirm(Locale.Prompts.BackupOculusDashExe);
        Logger.Log("Prompting to for folder");
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