using System.IO.Abstractions.TestingHelpers;
using BookmarkrV9.Commands.Import;
using BookmarkrV9.Models;
using BookmarkrV9.Services.BookmarkService;
using NSubstitute;

namespace Bookmarkr.UnitTests.Commands.Import;

[TestClass]
public class ImportCommandTests
{
    [TestMethod]
    public void OnImportCommand_PassingAValidAndExistingFile_CallsImportMethodOnBookmarkService()
    {
        // Arrange
        var mockBookmarkService = Substitute.For<IBookmarkService>();

        const string bookmarksAsJson = @"[
            {
                ""Name"": ""Packt Publishing"",
                ""Url"": ""https://packtpub.com/"",
                ""Category"": ""Tech Books""
            },
            {
                ""Name"": ""Audi Cars"",
                ""Url"": ""https://www.audi.com/"",
                ""Category"": ""See Later""
            },
            {
                ""Name"": ""LinkedIn"",
                ""Url"": ""https://www.linkedin.com/"",
                ""Category"": ""Social Media""
            }
        ]";

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {
                @"bookmarks.json", new MockFileData(bookmarksAsJson)
            }
        });

        var command = new ImportCommand(mockBookmarkService, mockFileSystem, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(mockFileSystem.FileInfo.New("bookmarks.json"));

        // Assert
        mockBookmarkService.Received(3).Import(Arg.Any<Bookmark>());
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "Packt Publishing" && bookmark.Url == "https://packtpub.com/"));
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "Audi Cars" && bookmark.Url == "https://www.audi.com/"));
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "LinkedIn" && bookmark.Url == "https://www.linkedin.com/"));
    }

    [TestMethod]
    public void OnImportCommand_Conflict_TheNameOfTheConflictingBookmarkIsUpdated()
    {
        // Arrange
        var bookmarkService = new BookmarkService();
        bookmarkService.ClearAll();
        bookmarkService.AddLink("Audi Canada", "https://audi.ca", "See Later");

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

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            {
                @"bookmarks.json", new MockFileData(bookmarksAsJson)
            }
        });

        var command = new ImportCommand(bookmarkService, mockFileSystem, "import", "Imports all bookmarks from a file");

        // Act
        command.OnImportCommand(mockFileSystem.FileInfo.New("bookmarks.json"));
        var currentBookmarks = bookmarkService.GetAll();

        // Assert
        Assert.AreEqual(3, currentBookmarks.Count);
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "Packt Publishing" && bookmark.Url == "https://packtpub.com/" && bookmark.Category == "Tech Books"));
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "Audi Cars" && bookmark.Url == "https://audi.ca" && bookmark.Category == "See Later"));
        Assert.IsTrue(currentBookmarks.Any(bookmark => bookmark.Name == "LinkedIn" && bookmark.Url == "https://www.linkedin.com/" && bookmark.Category == "Social Media"));
        Assert.IsFalse(currentBookmarks.Any(bookmark => bookmark.Name == "Audi Canada" && bookmark.Url == "https://audi.ca" && bookmark.Category == "See Later"));
    }
}
