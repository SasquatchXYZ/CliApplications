using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using BookmarkrV9.Commands.Export;
using BookmarkrV9.Commands.Import;
using BookmarkrV9.Commands.Interactive;
using BookmarkrV9.Commands.Link;
using BookmarkrV9.Commands.Sync;
using BookmarkrV9.Services.BookmarkService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BookmarkrV9;

class Program
{
    private static async Task<int> Main(string[] args)
    {
        FreeSerilogLoggerOnShutdown();

        IBookmarkService _bookmarkService;
        IHttpClientFactory _httpClientFactory;
        // The Root Command
        var rootCommand = new RootCommand("Bookmarkr is a bookmark manager provided as a CLI application.");

        rootCommand.SetHandler(OnHandleRootCommand);

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IBookmarkService, BookmarkService>();
                services.AddHttpClient("bookmarkrSyncr", client =>
                {
                    client.BaseAddress = new Uri("https://bookmarkrsyncr-api.azurewebsites.net");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "Bookmarkr");
                });
            })
            .Build();

        _bookmarkService = host.Services.GetRequiredService<IBookmarkService>();
        _httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();

        // Register Subcommands of the Root Command
        rootCommand.AddCommand(new ExportCommand(_bookmarkService, "export", "Exports all bookmarks to a designated output file"));
        rootCommand.AddCommand(new ImportCommand(_bookmarkService, "import", "Imports all bookmarks from a designated file"));
        rootCommand.AddCommand(new LinkCommand(_bookmarkService, "link", "Manage bookmarks links"));
        rootCommand.AddCommand(new InteractiveCommand(_bookmarkService, "interactive", "Manage bookmarks interactively"));
        rootCommand.AddCommand(new SyncCommand(_httpClientFactory, _bookmarkService, "sync", "Sync local and remote bookmark stores"));

        // The Builder Pattern
        var parser = new CommandLineBuilder(rootCommand)
            .UseHost(_ => Host.CreateDefaultBuilder(),
                builder =>
                {
                    builder.ConfigureServices(services =>
                    {
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

        // Handler for the Root Command
        static void OnHandleRootCommand()
        {
            Console.WriteLine("Hello from the root command!");
        }
    }

    private static void FreeSerilogLoggerOnShutdown()
    {
        // This event is raised when the process is about to exit,
        // allowing you to perform cleanup tasks or save data
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => ExecuteShutdownTasks();
        // This event is triggered when the user presses Ctrl+C or Ctrl+Break.
        // While it doesn't cover all shutdown scenarios,
        // it's useful for handling user-initiated terminations.
        Console.CancelKeyPress += (sender, eventArgs) => ExecuteShutdownTasks();
    }

    // Code to execute before shutdown
    private static void ExecuteShutdownTasks()
    {
        Console.WriteLine("Performing shutdown tasks...");
        Log.CloseAndFlush();
    }
}
