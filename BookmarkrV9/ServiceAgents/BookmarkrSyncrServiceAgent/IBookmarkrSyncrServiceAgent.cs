using BookmarkrV9.Models;

namespace BookmarkrV9.ServiceAgents.BookmarkrSyncrServiceAgent;

public interface IBookmarkrSyncrServiceAgent
{
    Task<List<Bookmark>> SyncBookmarks(List<Bookmark> localBookmarks);
}
