using System.Net;
using System.Text;
using System.Text.Json;
using BookmarkrV13.Models;

namespace BookmarkrV13.ServiceAgents.BookmarkrSyncrServiceAgent;

public class BookmarkrSyncrServiceAgent : IBookmarkrSyncrServiceAgent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BookmarkrSyncrServiceAgent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<Bookmark>> SyncBookmarks(string accessToken, List<Bookmark> localBookmarks)
    {
        // Ensure that the accessToken is present
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            var value = Environment.GetEnvironmentVariable("BOOKMARKR_ACCESS_TOKEN");
            if (value is null) throw new AccessTokenNotFoundException(accessToken);
            accessToken = value;
        }

        var serializedLocalBookmarks = JsonSerializer.Serialize(localBookmarks);
        var content = new StringContent(serializedLocalBookmarks, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient("bookmarkrSyncr");
        client.DefaultRequestHeaders.Add("X-PAT", accessToken);
        var response = await client.PostAsync("sync", content);

        if (response.IsSuccessStatusCode)
        {
            // Saving the accessToken to the environment variable, if not already present
            var value = Environment.GetEnvironmentVariable("BOOKMARKR_ACCESS_TOKEN");
            if (value is null || !value.Equals(accessToken))
                Environment.SetEnvironmentVariable("BOOKMARKR_ACCESS_TOKEN", accessToken);

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
                if (response.Headers.TryGetValues("X-Invalid-PAT", out _))
                    throw new AccessTokenInvalidException(accessToken);

                if (response.Headers.TryGetValues("X-Expired-PAT", out _))
                    throw new AccessTokenExpiredException(accessToken);

                throw new HttpRequestException($"Unauthorized access: {response.StatusCode}");
            default:
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to sync bookmarks: {response.StatusCode} {error}");
        }
    }
}
