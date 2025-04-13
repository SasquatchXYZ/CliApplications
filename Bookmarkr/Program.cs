using static Bookmarkr.Utilities.Helper;
using Bookmarkr.Services;

namespace Bookmarkr;

class Program
{
    private static void Main(string[]? args)
    {
        if (args is null || args.Length == 0)
        {
            ShowErrorMessage(["You haven't passed any argument.  The expected syntax is:", "bookmarkr <command-name> <parameters>"]);
            return;
        }

        var service = new BookmarkService();

        switch (args[0].ToLower())
        {
            case "link":
                ManageLinks(args, service);
                break;
            // We may add more commands here...
            default:
                ShowErrorMessage(["Unknown Command"]);
                break;
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
