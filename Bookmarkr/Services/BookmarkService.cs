using static Bookmarkr.Utilities.Helper;
using Bookmarkr.Models;

namespace Bookmarkr.Services;

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
    }

    public void RemoveLink(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowErrorMessage(["The 'name' for the link is not provided.  The expected syntax is:", "bookmarkr link add <name> <url>"]);
            return;
        }

        var index = _bookmarks.FindIndex(bookmark => bookmark.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
        {
            ShowWarningMessage([$"No bookmark with the name '{name}' exists."]);
            return;
        }

        _bookmarks.RemoveAt(index);
        ShowSuccessMessage(["Bookmark successfully removed."]);
    }
}
