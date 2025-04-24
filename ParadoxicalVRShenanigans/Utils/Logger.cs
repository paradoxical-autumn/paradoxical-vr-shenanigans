using Spectre.Console;
using System;
using System.Reflection;

namespace ParadoxVrTools;

public static class Logger
{
    private static string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    private static string logName = GenerateLogName(Utils.GetVersion());
    private static string logPath = exePath + $"\\logs\\{logName}";

    public static void Log(string message, bool stacktrace = false)
    {
        if (!Directory.Exists(exePath + "\\logs"))
        {
            Directory.CreateDirectory(exePath + "\\logs");
        }

        using (StreamWriter w = File.AppendText(logPath))
        {
            GenerateLogMessage(message, w, stacktrace);
        }
    }

    public static void Error(string message, bool stacktrace = true, bool nomarkup = false)
    {
        if (!Directory.Exists(exePath + "\\logs"))
        {
            Directory.CreateDirectory(exePath + "\\logs");
        }

        using (StreamWriter w = File.AppendText(logPath))
        {
            if (!nomarkup)
            {
                AnsiConsole.MarkupLine("[red]{0}[/]", $"{string.Format(Locale.Errors.Exception, message)}".EscapeMarkup());
            }
            GenerateLogMessage($"[ERROR!!] {message}", w, stacktrace);
        }
    }

    public static void ImproperExit(string message, bool stacktrace = true, int exitCode = 0)
    {
        if (!Directory.Exists(exePath + "\\logs"))
        {
            Directory.CreateDirectory(exePath + "\\logs");
        }

        using (StreamWriter w = File.AppendText(logPath))
        {
            GenerateLogMessage($"\n--- IMPROPERLY EXITING ---\n{message}\n---", w, stacktrace);
        }
        
        System.Environment.Exit(exitCode);
    }

    private static void GenerateLogMessage(string logMsg, TextWriter txtWriter, bool stacktrace)
    {
        logMsg = $"[{DateTime.Now.ToLongTimeString()}] {logMsg}";

        if (stacktrace)
        {
            logMsg = $"{logMsg} \n\nStacktrace:\n {Environment.StackTrace}";
        }

        txtWriter.WriteLine(logMsg);
    }

    public static string GenerateLogName(string versionNumber, string suffix = "")
    {
        return Environment.MachineName + " - " + versionNumber + " - " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + suffix + ".log";
    }
}