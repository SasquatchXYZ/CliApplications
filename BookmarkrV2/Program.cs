using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using BookmarkrV2.Services;

namespace BookmarkrV2;

class Program
{
    private static readonly BookmarkService _bookmarkService = new();

    private static async Task<int> Main(string[] args)
    {
        // The Root Command
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.")
        {
        };

        rootCommand.SetHandler(OnHandleRootCommand);

        // The Link Command
        var linkCommand = new Command("link", "Manage bookmarks links")
        {
        };

        rootCommand.AddCommand(linkCommand);

        // Add Options for the Link Command
        var nameOption = new Option<string>(
            ["--name", "-n"],
            "The name of the bookmark"
        )
        {
            IsRequired = true,
        };

        var urlOption = new Option<string>(
            ["--url", "-u"],
            "The Url of the bookmark"
        )
        {
            IsRequired = true,
        };

        var categoryOption = new Option<string>(
            ["--category", "-c"],
            "The category to which the bookmark is associated."
        )
        {
            IsRequired = false,
        };

        categoryOption.SetDefaultValue("Read later");

        // The Add Command
        var addLinkCommand = new Command("add", "Add a new bookmark link")
        {
            nameOption,
            urlOption,
            categoryOption,
        };

        linkCommand.AddCommand(addLinkCommand);

        addLinkCommand.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption, categoryOption);

        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);

        // Handler Methods
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }

        static void OnHandleAddLinkCommand(string name, string url, string category)
        {
            _bookmarkService.AddLink(name, url, category);
        }
    }
}
