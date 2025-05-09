using System.CommandLine;
using System.Net;
using System.Text;
using System.Text.Json;
using BookmarkrV9.Models;
using BookmarkrV9.ServiceAgents.BookmarkrSyncrServiceAgent;
using BookmarkrV9.Services.BookmarkService;
using Serilog;

namespace BookmarkrV9.Commands.Sync;

public class SyncCommand : Command
{

    #region Properties

    private readonly IBookmarkrSyncrServiceAgent _serviceAgent;
    private readonly IBookmarkService _bookmarkService;

    #endregion

    #region Constructor

    public SyncCommand(
        IBookmarkrSyncrServiceAgent serviceAgent,
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _serviceAgent = serviceAgent;
        _bookmarkService = bookmarkService;
        this.SetHandler(OnSyncCommand);
    }

    #endregion

    #region Handler method

    private async Task OnSyncCommand()
    {
        var localBookmarks = _bookmarkService.GetAll();
        try
        {
            var mergedBookmarks = await _serviceAgent.SyncBookmarks(localBookmarks);
            _bookmarkService.ClearAll();
            _bookmarkService.Import(mergedBookmarks);
            Log.Information("Successfully synced bookmarks");
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex.Message);
        }
    }

    #endregion

}
