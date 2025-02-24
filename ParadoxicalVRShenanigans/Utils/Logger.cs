using System;
using System.Reflection;

namespace ParadoxVrTools;

public static class Logger
{
    private static string exePath = string.Empty;
    private static string logName = GenerateLogName(Utils.GetVersion());

    public static void Log(string message, bool stacktrace = false)
    {
        exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        string logPath = exePath + $"\\logs\\{logName}";

        if (!Directory.Exists(exePath + "\\logs"))
        {
            Directory.CreateDirectory(exePath + "\\logs");
        }

        using (StreamWriter w = File.AppendText(logPath))
        {
            GenerateLogMessage(message, w, stacktrace);
        }
    }

    private static void GenerateLogMessage(string logMsg, TextWriter txtWriter, bool stacktrace)
    {
        if (stacktrace)
        {
            logMsg = logMsg + "\n\nStacktrace:\n" + Environment.StackTrace;
        }

        txtWriter.WriteLine(logMsg);
    }

    public static string GenerateLogName(string versionNumber, string suffix = "")
    {
        return Environment.MachineName + " - " + versionNumber + " - " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + suffix + ".log";
    }
}