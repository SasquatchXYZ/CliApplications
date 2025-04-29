using BookmarkrV13.Models;

namespace BookmarkrV13.ServiceAgents.BookmarkrSyncrServiceAgent;

public interface IBookmarkrSyncrServiceAgent
{
    Task<List<Bookmark>> SyncBookmarks(string accessToken, List<Bookmark> localBookmarks);
}
