using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ParadoxVrTools.Commands;
public class SteamVRHomeKillCommand : AsyncCommand<SteamVRHomeKillCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description(Locale.Descriptions.SteamPath)]
        [CommandArgument(0, "[PATH]")]
        [DefaultValue("C:\\Program Files (x86)\\Steam")]
        public string? SteamPath { get; set; }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var stm_pth = settings.SteamPath;
        var set_path = $"{stm_pth}/config/steamvr.vrsettings";
        FileAttributes attr;

        try
        {
            attr = File.GetAttributes(stm_pth!);
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine($"Invalid path");
            return 1;
        }

        try
        {
            attr = File.GetAttributes(set_path);
        }
        catch (FileNotFoundException)
        {
            AnsiConsole.MarkupLine($"Unable to load Steam VR settings.");
            return 1;
        }

        //StreamReader sr = new StreamReader(set_path);
        string raw_data = File.ReadAllText(set_path);

        AnsiConsole.MarkupLine($"[italic royalblue1]beginning data write[/]");

        try
        {
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(raw_data)!;

            try
            {
                jsonObj["steamvr"]["enableHomeApp"] = false;
            }
            catch (RuntimeBinderException)
            {
                //Dictionary<string, object> dict = new Dictionary<string, object>()
                //{
                //    { "enableHomeApp", false }
                //};
                //AnsiConsole.WriteLine(JsonConvert.SerializeObject(dict));
                jsonObj["steamvr"] = JObject.Parse("{\"enableHomeApp\": false}");
            }
            
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            //AnsiConsole.MarkupLine($"[royalblue1]{output}[/]");

            File.WriteAllText(set_path, output);
        }
        catch (Exception ex)
        {
            Utils.PrintErr(ex);
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[italic darkred]verifying file data...[/]");
            string new_dat = File.ReadAllText(set_path);

            if (new_dat == raw_data)
            {
                AnsiConsole.MarkupLine("[italic seagreen1]file verification success! [underline]your VR settings have not been edited.[/][/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[italic darkred]file data does not match. writing old data.[/]"); // OLD_DATA is not an inscryption reference. well it wasn't until I wrote this comment.
                File.WriteAllText(set_path, raw_data);

                new_dat = File.ReadAllText(set_path);
                if (new_dat == raw_data)
                {
                    AnsiConsole.MarkupLine("[italic seagreen1]file recovered! [underline]your VR settings have not been edited.[/][/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[italic darkred]file data [underline]still[/] does not match. please manually rewrite data.[/]");
                    AnsiConsole.WriteLine($"the data you need to rewrite will be displayed on screen\nplace it into the file: {set_path}\npress any key to continue.");
                    Console.ReadKey();
                    AnsiConsole.MarkupLine($"[blue]{raw_data}[/]");
                }
            }
            return 1;
        }

        //sr.Close();

        AnsiConsole.MarkupLine("[italic seagreen1]finished![/]");

        return 0;
    }
}