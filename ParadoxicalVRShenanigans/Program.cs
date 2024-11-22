using ParadoxVrTools.Commands;
using Spectre.Console.Cli;
using System.Text;

namespace ParadoxVrTools;
public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;
        System.Console.InputEncoding = Encoding.UTF8;

        var app = new CommandApp<DefaultCommand>();
        app.Configure(config =>
        {
            config.SetApplicationName(Locale.Application.AppName);
        });

        return await app.RunAsync(args);
    }
}