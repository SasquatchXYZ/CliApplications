using static Bookmarkr.Utilities.Helper;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Bookmarkr.Services;

namespace Bookmarkr;

class Program
{
    private static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.")
        {
        };

        rootCommand.SetHandler(OnHandleRootCommand);

        var parser = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .Build();

        return await parser.InvokeAsync(args);

        // if (args is null || args.Length == 0)
        // {
        //     ShowErrorMessage(["You haven't passed any argument.  The expected syntax is:", "bookmarkr <command-name> <parameters>"]);
        //     return;
        // }
        //
        // var service = new BookmarkService();
        //
        // switch (args[0].ToLower())
        // {
        //     case "link":
        //         ManageLinks(args, service);
        //         break;
        //     // We may add more commands here...
        //     default:
        //         ShowErrorMessage(["Unknown Command"]);
        //         break;
        // }
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }
    }

    private static void ManageLinks(string[] args, BookmarkService service)
    {
        if (args.Length < 2)
        {
            ShowErrorMessage(["Insufficient number of parameters.  The expected syntax is:", "bookmarkr link <subcommand> <parameters>"]);
        }

        switch (args[1].ToLower())
        {
            case "add":
                service.AddLink(args[2], args[3]);
                break;
            // We may add more subcommands here...
            default:
                ShowErrorMessage(["Insufficient number of parameters.  The expected syntax is:", "bookmarkr link <subcommand> <parameters>"]);
                break;
        }
    }
}
