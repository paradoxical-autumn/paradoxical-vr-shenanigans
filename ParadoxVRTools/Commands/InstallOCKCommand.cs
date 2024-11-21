using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ParadoxVrTools.Commands;
public class OCKCommand : AsyncCommand<OCKCommand.Settings>
{
    Random rnd = new Random();

    public class Settings : CommandSettings
    {
        [Description(Strings.Descriptions.BackupOCD)]
        [CommandOption("-b|--backup")]
        public bool BackupOCD { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        AnsiConsole.Clear();

        await AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn()
                {
                    CompletedStyle = new Style(Color.SeaGreen1),
                    RemainingStyle = new Style(Color.Grey15),
                    FinishedStyle = new Style(Color.SeaGreen1),
                },
                new PercentageColumn()
                {
                    CompletedStyle = new Style(Color.SeaGreen1)
                },
                new TextColumn()
                {
                    Text = "eta: "
                },
                new RemainingTimeColumn(),
                new SpinnerColumn(Spinner.Known.Point),
            })
            .StartAsync(async ctx =>
            {
                var task1 = ctx.AddTask("downloading EPIC HACKER CODE");
                var task2 = ctx.AddTask("installing SUPER HACKER CODE");

                while (!ctx.IsFinished)
                {
                    await Task.Delay(250);

                    task1.Increment(1.5);
                    task2.Increment(0.5);
                }
            });

        AnsiConsole.WriteLine("woah is complete?");

        return 0;
    }

    public sealed class TextColumn : ProgressColumn
    {
        public string Text { get; set; } = "/";

        public Style Style { get; set; } = Color.Blue;

        public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
        {
            return new Text(Text, Style ?? Style.Plain);
        }

        public override int? GetColumnWidth(RenderOptions options)
        {
            return Text.Length;
        }
    }
}