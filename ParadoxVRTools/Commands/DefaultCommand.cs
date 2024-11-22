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
            AnsiConsole.MarkupLine($"[red]{Strings.Errors.NotInteractiveConsole}[/]");
            return 1;
        }

        if (settings.PrintVersion)
        {
            AnsiConsole.Write(new FigletText("Paradox VR Tools"));

            string datastr = @"made with <3 (and tears) by autumn and is provided to whoever finds this note.

this was my first major project learning C# and i'd argue it turned out quite well. but i still need to thank you,
i dont get anything from uploading my code and i dont want anything in particular -- just to know that my code made someone's day.

anyway, i hope u enjoy this collection of random tools and i hope u find them helpful!
ttyl, see you in cyberspace.
";

            var pnl = new Panel(datastr)
                .RoundedBorder()
                .BorderColor(Color.SeaGreen3)
                .Header("NOTE FROM PARADOX.txt");

            AnsiConsole.Write(pnl);

            var table = new Table();
            table.BorderColor(Color.SeaGreen3);


            table.AddColumn("METADATA").Centered();
            table.AddColumn("VALUE").Centered();

            table.AddRow("Version", Strings.Meta.version.ToString());
            table.AddRow("Compiled by", Strings.Meta.LastCompiler);

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
                    case Strings.MenuOptions.DisableSteamVRHome:
                        AnsiConsole.Clear();

                        AnsiConsole.WriteLine();
                        await new SteamVRHomeKillCommand().ExecuteAsync(context, PromptForDSVRHSettings());
                        break;
                    case Strings.MenuOptions.InstallOculusKiller:
                        AnsiConsole.Clear();
                        AnsiConsole.WriteLine();

                        var new_settings = PromptForOCKSettings();
                        if (new_settings == null)
                        {
                            break;
                        }

                        await new OCKCommand().ExecuteAsync(context, new_settings);
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

        if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
        {
            Panel pnl = Strings.NotStrings.WrongOS();
            AnsiConsole.Write(pnl);
        }
    }

    private string ShowMainMenu()
    {
        var prompt = new SelectionPrompt<string>()
                .Title(Strings.Prompts.InteractionPrompt)
                .HighlightStyle(new Style(foreground: Color.SeaGreen1, decoration: Decoration.Bold))
                .AddChoices(new[] {
                            Strings.MenuOptions.DisableSteamVRHome,
                            Strings.MenuOptions.InstallOculusKiller,
                            Strings.MenuOptions.Quit
                });

        prompt.HighlightStyle = new Style(Color.Black, Color.SeaGreen1 );
        prompt.SearchHighlightStyle = new Style(Color.Black, Color.MediumOrchid1 );
        prompt.WrapAround = true;

        return AnsiConsole.Prompt(prompt);
    }

    private SteamVRHomeKillCommand.Settings PromptForDSVRHSettings()
    {
        string stm_pth = AnsiConsole.Prompt<string>(new TextPrompt<string>(Strings.Prompts.EnterSteamFolderPath).AllowEmpty());

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
            AnsiConsole.MarkupLine($"[red]{Strings.Errors.RequiresElevation}[/]");
            return null;
        }

        bool backup = AnsiConsole.Confirm(Strings.Prompts.BackupOculusDashExe);
        string oc_pth = AnsiConsole.Prompt<string>(new TextPrompt<string>(Strings.Prompts.EnterOculusFolderPath).AllowEmpty());

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