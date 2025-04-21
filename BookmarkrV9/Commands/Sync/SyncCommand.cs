using System.CommandLine;
using System.Net;
using System.Text;
using System.Text.Json;
using BookmarkrV9.Models;
using BookmarkrV9.Services.BookmarkService;
using Serilog;

namespace BookmarkrV9.Commands.Sync;

public class SyncCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Constructor

    public SyncCommand(
        IHttpClientFactory httpClientFactory,
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _httpClientFactory = httpClientFactory;
        _bookmarkService = bookmarkService;
        this.SetHandler(OnSyncCommand);
    }

    #endregion

    #region Handler method

    private async Task OnSyncCommand()
    {
        var localBookmarks = _bookmarkService.GetAll();
        var serializedLocalBookmarks = JsonSerializer.Serialize(localBookmarks);
        var content = new StringContent(serializedLocalBookmarks, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient("bookmarkrSyncr");
        var response = await client.PostAsync("sync", content);

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var mergedBookmarks = await JsonSerializer.DeserializeAsync<List<Bookmark>>(
                await response.Content.ReadAsStreamAsync(), options);

            _bookmarkService.ClearAll();
            _bookmarkService.Import(mergedBookmarks);

            Log.Information("Successfully synced bookmarks");
        }
        else
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    Log.Error("Resource not found");
                    break;
                case HttpStatusCode.Unauthorized:
                    Log.Error("Unauthorized access");
                    break;
                default:
                    var error = await response.Content.ReadAsStringAsync();
                    Log.Error($"Failed to sync bookmarks | {error}");
                    break;
            }
        }
    }

    #endregion

}
