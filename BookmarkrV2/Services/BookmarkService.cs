using static BookmarkrV2.Utilities.Helper;
using BookmarkrV2.Models;

namespace BookmarkrV2.Services;

public class BookmarkService
{
    private readonly List<Bookmark> _bookmarks = [];

    public void AddLink(string name, string url)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowErrorMessage(["The 'name' for the link is not provided.  The expected syntax is:", "bookmarkr link add <name> <url>"]);
            return;
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            ShowErrorMessage(["The 'url' for the link is not provided.  The expected syntax is:", "bookmarkr link add <name> <url>"]);
            return;
        }

        if (_bookmarks.Any(bookmark => bookmark.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            ShowWarningMessage([
                $"A link with the name '{name}' already exists.  It will not be added", $"To update the existing link, use the command: bookmarkr link update '{name}' '{url}'"
            ]);

            return;
        }

        _bookmarks.Add(new Bookmark
        {
            Name = name,
            Url = url
        });

        ShowSuccessMessage(["Bookmark added successfully."]);
        Console.WriteLine(_bookmarks.Count);
    }
}
