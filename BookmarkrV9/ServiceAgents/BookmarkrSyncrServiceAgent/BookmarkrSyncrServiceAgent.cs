using System.Net;
using System.Text;
using System.Text.Json;
using BookmarkrV9.Models;

namespace BookmarkrV9.ServiceAgents.BookmarkrSyncrServiceAgent;

public class BookmarkrSyncrServiceAgent : IBookmarkrSyncrServiceAgent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BookmarkrSyncrServiceAgent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<Bookmark>> SyncBookmarks(List<Bookmark> localBookmarks)
    {
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

            return mergedBookmarks;
        }

        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new HttpRequestException($"Resource not found: {response.StatusCode}");
            case HttpStatusCode.Unauthorized:
                throw new HttpRequestException($"Unauthorized access: {response.StatusCode}");
            default:
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to sync bookmarks: {response.StatusCode} {error}");
        }
    }
}
