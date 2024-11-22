using Spectre.Console;

namespace ParadoxVrTools;

public static class Locale
{
    public static class Meta
    {
        public const string LastCompiler = "paradoxical-autumn";
    }

    public static class Application
    {
        public static string AppName = "Paradoxical VR Shenanigans";
    }

    public static class Prompts
    {
        public const string InteractionPrompt = "What would you like to do?";
        public const string ReturnToMainMenu = "Return to main menu?";
        public const string EnterSteamFolderPath = "Enter the path to your steam installation folder (contains steam.exe)\nOr, leave blank for default: ";
        public const string EnterOculusFolderPath = "Enter the path to your oculus installation folder (contains steam.exe)\nOr, leave blank for default: ";
        public const string HaveCustomisedSteamPath = "Did you install steam to the default directory?";
        public const string BackupOculusDashExe = "Do you want to create a backup of oculus dash?";
        public const string UpdateAvailable = "There is an update available! Lucky you! Would you like to update?";
        public const string RebootOVR = "Would you like to reboot OVRService so you don't have to reboot your PC?";
    }

    public static class Messages
    {
        public const string PressKeyExit = "Press any key to exit.";
    }

    public static class Errors
    {
        public const string SteamNotFound = "I couldn't find steam's installation directory.";
        public const string Exception = "An error occurred: {0}";
        public const string NotInteractiveConsole = "Console environment doesn't support interaction.";
        public const string RequiresElevation = "You need to be running as an administrator to use this.";
    }

    public static class Statuses
    {
        public const string LoadingSettings = "Loading settings...";
        public const string Starting = "Starting...";
    }

    public static class MenuOptions
    {
        public const string DisableSteamVRHome = "Disable SteamVR Home";
        public const string InstallOculusKiller = "Install OculusKiller";
        public const string Quit = "Quit";
    }

    public static class Descriptions
    {
        public const string DisableSteamVRHome = "Disables SteamVR home";
        public const string InstallOCK = "Installs Oculus Killer, an app that replaces Oculus Dash with SteamVR";
        public const string SteamPath = "The path where steam is installed to";
        public const string BackupOCD = "If Oculus Dash should be backed up or not";
        public const string OculusPath = "The path where Oculus is installed to";
    }

    public static class NotStrings
    {
        public static Panel WrongOS()
        {
            string datastr = @"[underline]You are using an unsupported OS.[/]
You may experience weirdness and will definitely have to deal with:
- Incorrect default paths
- Missing functionality (sudo)
- Other jank";

            Panel pnl = new Panel(datastr);
            pnl.RoundedBorder().BorderColor(Color.Red).Header("WARNING");

            return pnl;
        }

        public static Panel SecretMenu()
        {
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

            return pnl;
        }
    }
}