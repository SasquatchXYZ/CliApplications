using BookmarkrV12.Models;

namespace BookmarkrV12.ServiceAgents.BookmarkrSyncrServiceAgent;

public interface IBookmarkrSyncrServiceAgent
{
    Task<List<Bookmark>> SyncBookmarks(List<Bookmark> localBookmarks);
}
