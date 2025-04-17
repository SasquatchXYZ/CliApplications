using static BookmarkrV3.Utilities.Helper;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Text.Json;
using BookmarkrV3.Models;
using BookmarkrV3.Services;
using Microsoft.Extensions.Hosting;

namespace BookmarkrV3;

class Program
{
    private static readonly BookmarkService _bookmarkService = new();

    private static async Task<int> Main(string[] args)
    {
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

        var parser = new CommandLineBuilder(rootCommand)
            .UseHost(_ => Host.CreateDefaultBuilder(),
                host => { host.ConfigureServices(services => { }); })
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
            _bookmarkService.Import(bookmarks);
        }
    }
}
