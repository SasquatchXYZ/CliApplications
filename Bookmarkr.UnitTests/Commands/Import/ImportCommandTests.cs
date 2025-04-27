using System.IO.Abstractions.TestingHelpers;
using BookmarkrV12.Commands.Import;
using BookmarkrV12.Models;
using BookmarkrV12.Services.BookmarkService;
using NSubstitute;

namespace Bookmarkr.UnitTests.Commands.Import;

[TestClass]
public class ImportCommandTests
{
    public required IBookmarkService _bookmarkService;
    public required MockFileSystem _fileSystemMock;

    [TestInitialize]
    public void TestInitialize()
    {
        const string bookmarksAsJson = @"[
            {
                ""Name"": ""Packt Publishing"",
                ""Url"": ""https://packtpub.com/"",
                ""Category"": ""Tech Books""
            },
            {
                ""Name"": ""Audi Cars"",
                ""Url"": ""https://audi.ca"",
                ""Category"": ""See Later""
            },
            {
                ""Name"": ""LinkedIn"",
                ""Url"": ""https://www.linkedin.com/"",
                ""Category"": ""Social Media""
            }
        ]";

        _fileSystemMock = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {
                @"bookmarks.json", new MockFileData(bookmarksAsJson)
            }
        });
    }

    [TestMethod]
    public void OnImportCommand_PassingAValidAndExistingFile_CallsImportMethodOnBookmarkService()
    {
        // Arrange
        _bookmarkService = Substitute.For<IBookmarkService>();


        var command = new ImportCommand(_bookmarkService, _fileSystemMock, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(_fileSystemMock.FileInfo.New("bookmarks.json"));

        // Assert
        _bookmarkService.Received(3).Import(Arg.Any<Bookmark>());
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "Packt Publishing" && bookmark.Url == "https://packtpub.com/"));
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "Audi Cars" && bookmark.Url == "https://audi.ca"));
        _bookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "LinkedIn" && bookmark.Url == "https://www.linkedin.com/"));
    }

    [TestMethod]
    public void OnImportCommand_Conflict_TheNameOfTheConflictingBookmarkIsUpdated()
    {
        // Arrange
        _bookmarkService = new BookmarkService();
        _bookmarkService.ClearAll();
        _bookmarkService.AddLink("Audi Canada", "https://audi.ca", "See Later");

        var command = new ImportCommand(_bookmarkService, _fileSystemMock, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(_fileSystemMock.FileInfo.New("bookmarks.json"));
        var currentBookmarks = _bookmarkService.GetAll();

        // Assert
        Assert.AreEqual(3, currentBookmarks.Count);
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "Packt Publishing" && bookmark.Url == "https://packtpub.com/" && bookmark.Category == "Tech Books"));
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "Audi Cars" && bookmark.Url == "https://audi.ca" && bookmark.Category == "See Later"));
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "LinkedIn" && bookmark.Url == "https://www.linkedin.com/" && bookmark.Category == "Social Media"));
        Assert.IsFalse(currentBookmarks.Any(bookmark => bookmark.Name == "Audi Canada" && bookmark.Url == "https://audi.ca" && bookmark.Category == "See Later"));
    }
}
