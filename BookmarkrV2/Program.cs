using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Text.Json;
using BookmarkrV2.Models;
using BookmarkrV2.Services;

namespace BookmarkrV2;

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
        exportCommand.SetHandler(OnExportCommand, outputFileOption);

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

        static void OnExportCommand(FileInfo outputFile)
        {
            var bookmarks = _bookmarkService.GetAll();
            var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(outputFile.FullName, json);
        }

        static void OnImportCommand(FileInfo inputFile)
        {
            var json = File.ReadAllText(inputFile.FullName);
            var bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? [];
            _bookmarkService.Import(bookmarks);
        }
    }
}
