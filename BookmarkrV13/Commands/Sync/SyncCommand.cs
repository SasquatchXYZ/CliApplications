using static BookmarkrV13.Utilities.Helper;
using System.CommandLine;
using BookmarkrV13.ServiceAgents.BookmarkrSyncrServiceAgent;
using BookmarkrV13.Services.BookmarkService;
using Serilog;

namespace BookmarkrV13.Commands.Sync;

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
        AddOption(patOption);
        _serviceAgent = serviceAgent;
        _bookmarkService = bookmarkService;
        this.SetHandler(OnSyncCommand, patOption);
    }

    #endregion

    #region Options

    private Option<string> patOption = new(
        ["--pat", "-p"],
        "The personal access token used to authenticate to BookmarkrSyncr"
    );

    #endregion

    #region Handler method

    private async Task OnSyncCommand(string patValue)
    {
        var localBookmarks = _bookmarkService.GetAll();
        try
        {
            var mergedBookmarks = await _serviceAgent.SyncBookmarks(patValue, localBookmarks);
            _bookmarkService.ClearAll();
            _bookmarkService.Import(mergedBookmarks);
            Log.Information("Successfully synced bookmarks");
        }
        catch (AccessTokenNotFoundException ex)
        {
            ShowErrorMessage([$"The provided access token value ({ex.AccessToken}) was not found."]);
        }
        catch (AccessTokenInvalidException ex)
        {
            ShowErrorMessage([$"The provided access token value ({ex.AccessToken}) is invalid."]);
        }
        catch (AccessTokenExpiredException ex)
        {
            ShowErrorMessage([$"The provided access token value ({ex.AccessToken}) is expired."]);
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex.Message);
        }
    }

    #endregion

}
