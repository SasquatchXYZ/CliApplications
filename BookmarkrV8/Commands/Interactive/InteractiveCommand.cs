using System.CommandLine;
using System.Text.Json;
using BookmarkrV8.Services.BookmarkService;
using Spectre.Console;

namespace BookmarkrV8.Commands.Interactive;

public class InteractiveCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;

    #endregion

    #region Constructor

    public InteractiveCommand(
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;
        this.SetHandler(OnInteractiveCommand);
    }

    #endregion

    #region Handler method

    private void OnInteractiveCommand()
    {
        var isRunning = true;
        while (isRunning)
        {
            AnsiConsole.Write(
                new FigletText("Bookmarkr")
                    .Centered()
                    .Color(Color.SteelBlue)
            );

            var selectedOperation = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What do you want to do?[/]")
                    .AddChoices([
                        "Export Bookmarks To File",
                        "View Bookmarks",
                        "Exit Program"
                    ])
            );

            switch (selectedOperation)
            {
                case "Export Bookmarks To File":
                    ExportBookmarks();
                    break;
                case "View Bookmarks":
                    ViewBookmarks();
                    break;
                default:
                    isRunning = false;
                    break;
            }
        }
    }

    private void ExportBookmarks()
    {
        // Ask for the outputFilePath
        var outputFilePath = AnsiConsole.Prompt(
            new TextPrompt<string>("Please provide the output file name (default: 'bookmarks.json')")
                .DefaultValue("bookmarks.json"));

        // Export the bookmarks to the specified file, while showing progress.
        AnsiConsole.Progress()
            .AutoRefresh(true) // Turns on auto refresh
            .AutoClear(false) // Avoids removing the task list when completed
            .HideCompleted(false) // Avoids hiding tasks as they are completed
            .Columns(
            [
                new TaskDescriptionColumn(), // Shows the task description
                new ProgressBarColumn(), // Shows the progress bar
                new PercentageColumn(), // Shows the current percentage
                new RemainingTimeColumn(), // Shows the remaining time
                new SpinnerColumn(), // Shows the spinner, indicating that the operation is ongoing
            ])
            .Start(ctx =>
            {
                // Get the list of all bookmarks
                var bookmarks = _bookmarkService.GetAll();

                // Export the bookmarks to file
                // 1. Create the task
                var task = ctx.AddTask("[yellow]Exporting all bookmarks to file...[/]");

                // 2. Set the total steps for the progress bar
                task.MaxValue = bookmarks.Count;

                // 3. Open the file for writing
                using (var writer = new StreamWriter(outputFilePath))
                {
                    while (!ctx.IsFinished)
                    {
                        foreach (var bookmark in bookmarks)
                        {
                            // 3.1 Serialize the current bookmark as JSON and write to the file asynchronously
                            writer.WriteLine(JsonSerializer.Serialize(bookmark));

                            // 3.2. Increment the progress bar
                            task.Increment(1);

                            // 3.3. Slow down the process so we can see the progress...
                            Thread.Sleep(1500);
                        }
                    }
                }
            });

        AnsiConsole.MarkupLine("[green]All bookmarks have been successfully exported.[/]");
    }

    private void ViewBookmarks()
    {
        // Create the tree
        var root = new Tree("Bookmarks");

        // Add some nodes
        var readLaterCategory = root.AddNode("[yellow]Read Later[/]");
        var techBooksCategory = root.AddNode("[yellow]Tech Books[/]");
        var carsCategory = root.AddNode("[yellow]Cars[/]");
        var socialMediaCategory = root.AddNode("[yellow]Social Media[/]");
        var cookingCategory = root.AddNode("[yellow]Cooking[/]");

        var readLaterBooks = _bookmarkService.GetBookmarksByCategory("Read Later");
        foreach (var readLaterBook in readLaterBooks)
        {
            readLaterCategory.AddNode($"{readLaterBook.Name} | {readLaterBook.Url}");
        }

        var techBooks = _bookmarkService.GetBookmarksByCategory("Tech Books");
        foreach (var techBook in techBooks)
        {
            techBooksCategory.AddNode($"{techBook.Name} | {techBook.Url}");
        }

        var carsBooks = _bookmarkService.GetBookmarksByCategory("Cars");
        foreach (var carsBook in carsBooks)
        {
            carsCategory.AddNode($"{carsBook.Name} | {carsBook.Url}");
        }

        var socialMediaBooks = _bookmarkService.GetBookmarksByCategory("Social Media");
        foreach (var socialMediaBook in socialMediaBooks)
        {
            socialMediaCategory.AddNode($"{socialMediaBook.Name} | {socialMediaBook.Url}");
        }

        var cookingBooks = _bookmarkService.GetBookmarksByCategory("Cooking");
        foreach (var cookingBook in cookingBooks)
        {
            cookingCategory.AddNode($"{cookingBook.Name} | {cookingBook.Url}");
        }

        // Render the tree
        AnsiConsole.Write(root);
    }

    #endregion

}
