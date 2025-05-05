using System.CommandLine;
using BookmarkrV13.Commands.Link.Add;
using BookmarkrV13.Services.BookmarkService;

namespace BookmarkrV13.Commands.Link;

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
