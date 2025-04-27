using Spectre.Console;

namespace BookmarkrV12.Utilities;

static class Helper
{
    public static void ShowErrorMessage(string[] errorMessages)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.MarkupLine(
            Emoji.Known.CrossMark + " [bold red]ERROR[/] :cross_mark:"
        );

        foreach (var message in errorMessages)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]{message}[/]");
        }
    }

    public static void ShowWarningMessage(string[] warningMessages)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var m = new Markup(
            Emoji.Known.Warning + " [bold yellow]Warning[/] :warning:"
        );

        m.Centered();
        AnsiConsole.Write(m);
        AnsiConsole.WriteLine();
        foreach (var message in warningMessages)
        {
            AnsiConsole.MarkupLineInterpolated(
                $"[yellow]{message}[/]"
            );
        }
    }

    public static void ShowSuccessMessage(string[] successMessage)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        AnsiConsole.MarkupLine(
            Emoji.Known.BeatingHeart + " [bold green]SUCCESS[/] :beating_heart:"
        );

        foreach (var message in successMessage)
        {
            AnsiConsole.MarkupLineInterpolated($"[green]{message}[/]");
        }
    }
}
