using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Bookmarkr.Services;

namespace Bookmarkr;

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

        var listOption = new Option<string>(
            ["--list", "-l"],
            "Print all saved bookmarks"
        )
        {
            Arity = ArgumentArity.Zero,
        };

        // The Link Command
        var linkCommand = new Command("link", "Manage bookmarks links")
        {
            listOption
        };

        rootCommand.AddCommand(linkCommand);
        linkCommand.SetHandler(OnHandleListLinksCommand);

        // Add Options for the Link Command
        var nameOption = new Option<string>(
            ["--name", "-n"],
            "The name of the bookmark"
        );

        var urlOption = new Option<string>(
            ["--url", "-u"],
            "The Url of the bookmark"
        );

        // The Add Command
        var addLinkCommand = new Command("add", "Add a new bookmark link")
        {
            nameOption,
            urlOption
        };

        linkCommand.AddCommand(addLinkCommand);

        addLinkCommand.SetHandler(OnHandleAddLinkCommand, nameOption, urlOption);

        // The Remove Command
        var removeLinkCommand = new Command("remove", "Removes a bookmark link by name")
        {
            nameOption
        };

        linkCommand.AddCommand(removeLinkCommand);

        removeLinkCommand.SetHandler(OnHandleRemoveLinkCommand, nameOption);

        // The Update Command
        var updateLinkCommand = new Command("update", "Update a bookmark link")
        {
            nameOption,
            urlOption
        };

        linkCommand.AddCommand(updateLinkCommand);
        updateLinkCommand.SetHandler(OnHandleUpdateLinkCommand, nameOption, urlOption);

        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);

        // Handler Methods
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }

        static void OnHandleAddLinkCommand(string name, string url)
        {
            _bookmarkService.AddLink(name, url);
        }

        static void OnHandleRemoveLinkCommand(string name)
        {
            _bookmarkService.RemoveLink(name);
        }

        static void OnHandleUpdateLinkCommand(string name, string url)
        {
            _bookmarkService.UpdateLink(name, url);
        }

        static void OnHandleListLinksCommand()
        {
            _bookmarkService.ListLinks();
        }
    }
}
