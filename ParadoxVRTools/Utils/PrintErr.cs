using Spectre.Console;

namespace ParadoxVrTools;
public static class Utils
{
    public static void PrintErr(object obj)
    {
        string s = (obj != null) ? obj.ToString()! : "NULL";
        try
        {
            AnsiConsole.MarkupLine($"[red]{string.Format(Strings.Errors.Exception, s)}[/]");
        }
        catch (Exception e)
        {
            AnsiConsole.WriteLine($"Exception happened while handling exception:\n{e}");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine($"Original exception follows:\n{s}");
        }
    }
}