using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Text.Json;
using BookmarkrV7.Models;
using BookmarkrV7.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spectre.Console;
using static BookmarkrV7.Utilities.Helper;

namespace BookmarkrV7;

class Program
{
    private static readonly BookmarkService _bookmarkService = new();

    private static async Task<int> Main(string[] args)
    {
        FreeSerilogLoggerOnShutdown();

        // The Root Command
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.");

        rootCommand.SetHandler(OnHandleRootCommand);

        // The Link Command
        var linkCommand = new Command("link", "Manage bookmarks links");

        rootCommand.AddCommand(linkCommand);

        // Add Options for the Link Command
        var nameOption = new Option<string[]>(
            ["--name", "-n"],
            "The name of the bookmark"
        )
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore,
        };

        var urlOption = new Option<string[]>(
            ["--url", "-u"],
            "The Url of the bookmark"
        )
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore,
        };

        urlOption.AddValidator(result =>
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = "The Url is required";
            }
            else if (!Uri.TryCreate(result.Tokens[0].Value, UriKind.Absolute, out _))
            {
                result.ErrorMessage = "The Url is invalid";
            }
        });

        var categoryOption = new Option<string[]>(
            ["--category", "-c"],
            "The category to which the bookmark is associated."
        )
        {
            IsRequired = false,
            AllowMultipleArgumentsPerToken = true,
            Arity = ArgumentArity.OneOrMore,
        };

        categoryOption.SetDefaultValue("Read Later");
        categoryOption.FromAmong("Read Later", "Tech Books", "Cooking", "Social Media");
        categoryOption.AddCompletions("Read Later", "Tech Books", "Cooking", "Social Media");

        // The Add Command
        var addLinkCommand = new Command("add", "Add a new bookmark link")
        {
            nameOption,
            urlOption,
            categoryOption,
        };

        linkCommand.AddCommand(addLinkCommand);

        addLinkCommand.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption, categoryOption);

        var outputFileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The output file that will store the bookmarks"
        )
        {
            IsRequired = true,
        };

        outputFileOption.LegalFileNamesOnly();

        var exportCommand = new Command("export", "Exports all bookmarks to a designated output file")
        {
            outputFileOption,
        };

        rootCommand.AddCommand(exportCommand);
        exportCommand.SetHandler(async (context) =>
        {
            var outputFileOptionValue = context.ParseResult.GetValueForOption(outputFileOption);
            var cancellationToken = context.GetCancellationToken();
            await OnExportCommand(outputFileOptionValue, cancellationToken);
        });

        var inputFileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The input file that contains the bookmarks to be imported"
        )
        {
            IsRequired = true,
        };

        inputFileOption.LegalFileNamesOnly();
        inputFileOption.ExistingOnly();

        var importCommand = new Command("import", "Imports all bookmarks from a designated file")
        {
            inputFileOption,
        };

        rootCommand.AddCommand(importCommand);
        importCommand.SetHandler(OnImportCommand, inputFileOption);

        var interactiveCommand = new Command("interactive", "Manage bookmarks interactively");
        rootCommand.AddCommand(interactiveCommand);
        interactiveCommand.SetHandler(OnInteractiveCommand);

        var parser = new CommandLineBuilder(rootCommand)
            .UseHost(_ => Host.CreateDefaultBuilder(),
                host =>
                {
                    host.ConfigureServices(services =>
                    {
                        // ** Configuration in Code **
                        // services.AddSerilog(config =>
                        // {
                        //     config.MinimumLevel.Information();
                        //     config.WriteTo.Console();
                        //     config.WriteTo.File("logs/bookmarkr-.txt",
                        //         rollingInterval: RollingInterval.Day,
                        //         restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error);
                        //
                        //     config.CreateLogger();
                        // });

                        // ** Configuration from appsettings.json **
                        services.AddSerilog(config =>
                        {
                            var configuration = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json")
                                .Build();

                            config.ReadFrom.Configuration(configuration);
                        });
                    });
                })
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);

        // Handler Methods
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }

        static void OnHandleAddLinkCommand(string[] names, string[] urls, string[] categories)
        {
            _bookmarkService.AddLinks(names, urls, categories);
            _bookmarkService.ListAll();
        }

        static async Task OnExportCommand(FileInfo outputFile, CancellationToken cancellationToken)
        {
            try
            {
                var bookmarks = _bookmarkService.GetAll();
                var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                await File.WriteAllTextAsync(outputFile.FullName, json, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                var requested = ex.CancellationToken.IsCancellationRequested
                    ? "Cancellation was requested by you."
                    : "Cancellation was NOT requested by you.";

                ShowWarningMessage(["Operation was cancelled.", requested, $"Cancellation reason: {ex.Message}"]);
            }
            catch (JsonException ex)
            {
                ShowErrorMessage([$"Failed to serialize bookmarks to JSON.", $"Error message {ex.Message}"]);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage([$"Insufficient permissions to access the file {outputFile.FullName}.", $"Error message {ex.Message}"]);
            }
            catch (DirectoryNotFoundException ex)
            {
                ShowErrorMessage([$"The file {outputFile.FullName} cannot be found due to an invalid path.", $"Error message {ex.Message}"]);
            }
            catch (PathTooLongException ex)
            {
                ShowErrorMessage([$"The provided path is exceeding the maximum length.", $"Error message {ex.Message}"]);
            }
            catch (Exception ex)
            {
                ShowErrorMessage([$"An unknown exception occurred.", $"Error message {ex.Message}"]);
            }
        }

        static void OnImportCommand(FileInfo inputFile)
        {
            var json = File.ReadAllText(inputFile.FullName);
            var bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? [];

            foreach (var bookmark in bookmarks)
            {
                var conflict = _bookmarkService.Import(bookmark);
                if (conflict is not null)
                {
                    Log.Information(
                        "{TimeStamp} | Bookmark updated | name changed from '{ConflictOldName}' to '{ConflictNewName}' for Url '{ConflictUrl}'",
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        conflict.OldName,
                        conflict.NewName,
                        conflict.Url);
                }
            }
        }

        static void OnInteractiveCommand()
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

        static void ExportBookmarks()
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
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
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

        static void ViewBookmarks()
        {
            // Create the tree
            var root = new Tree("Bookmarks");

            // Add some nodes
            var techBooksCategory = root.AddNode("[yellow]Tech Books[/]");
            var carsCategory = root.AddNode("[yellow]Cars[/]");
            var socialMediaCategory = root.AddNode("[yellow]Social Media[/]");
            var cookingCategory = root.AddNode("[yellow]Cooking[/]");

            // Add bookmarks for the Tech Books category
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
    }

    private static void FreeSerilogLoggerOnShutdown()
    {
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => ExecuteShutdownTasks();
        Console.CancelKeyPress += (sender, eventArgs) => ExecuteShutdownTasks();
    }

    private static void ExecuteShutdownTasks()
    {
        Console.WriteLine("Performing shutdown tasks...");
        Log.CloseAndFlush();
    }
}
