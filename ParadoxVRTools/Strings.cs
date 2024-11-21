namespace ParadoxVrTools;

public static class Strings
{
    public static class Application
    {
        public static string AppName = "Paradox VR Tools";
    }

    public static class Prompts
    {
        public const string InteractionPrompt = "What would you like to do?";
        public const string ReturnToMainMenu = "Return to main menu?";
        public const string EnterSteamFolderPath = "Enter the path to your steam installation folder (contains steam.exe)\nOr, leave blank for default: ";
        public const string HaveCustomisedSteamPath = "Did you install steam to the default directory?";
        public const string BackupOculusDashExe = "Do you want to create a backup of oculus dash?";
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
    }

    public static class Commands
    {
        public const string NoSteamHome = "";
    }
}