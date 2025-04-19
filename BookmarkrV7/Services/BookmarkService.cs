using BookmarkrV7.Models;
using static BookmarkrV7.Utilities.Helper;

namespace BookmarkrV7.Services;

public class BookmarkService
{
    private readonly List<Bookmark> _bookmarks = [];

    public void AddLink(string name, string url, string category)
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
            Url = url,
            Category = category,
        });

        ShowSuccessMessage(["Bookmark added successfully."]);
        Console.WriteLine(_bookmarks.Count);
    }

    public void AddLinks(string[] names, string[] urls, string[] categories)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (!_bookmarks.Any(bookmark => bookmark.Name.Equals(names[i], StringComparison.OrdinalIgnoreCase)))
            {
                _bookmarks.Add(new Bookmark
                {
                    Name = names[i],
                    Url = urls[i],
                    Category = categories[i]
                });

                ShowSuccessMessage(["Bookmark added successfully."]);
                Console.WriteLine(_bookmarks.Count);
            }
        }
    }

    public void ListAll()
    {
        _bookmarks.ForEach(bookmark => Console.WriteLine($"Name: '{bookmark.Name}' | Url: '{bookmark.Url}' | Category: '{bookmark.Category}'"));
    }

    public List<Bookmark> GetAll()
    {
        return _bookmarks.ToList();
    }

    public void Import(List<Bookmark> bookmarks)
    {
        var count = 0;
        foreach (var bookmark in bookmarks)
        {
            _bookmarks.Add(bookmark);
            count++;
        }

        ShowSuccessMessage([$"Successfully imported {count} bookmarks."]);
    }

    public BookmarkConflictModel? Import(Bookmark bookmark)
    {
        var conflict = _bookmarks.FirstOrDefault(b =>
            b.Url.Equals(bookmark.Url, StringComparison.OrdinalIgnoreCase) &&
            b.Name.Equals(bookmark.Name, StringComparison.OrdinalIgnoreCase));

        if (conflict is not null)
        {
            var conflictModel = new BookmarkConflictModel
            {
                OldName = conflict.Name,
                NewName = bookmark.Name,
                Url = bookmark.Url
            };

            conflict.Name = bookmark.Name;
            return conflictModel;
        }

        _bookmarks.Add(bookmark);
        return null;
    }
}
