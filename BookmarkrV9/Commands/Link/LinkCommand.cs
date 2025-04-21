using System.CommandLine;
using BookmarkrV9.Commands.Link.Add;
using BookmarkrV9.Services.BookmarkService;

namespace BookmarkrV9.Commands.Link;

public class LinkCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;

    #endregion

    #region Constructor

    public LinkCommand(
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;
        AddCommand(new LinkAddCommand(_bookmarkService, "add", "Add a new bookmark link"));
    }

    #endregion

}
