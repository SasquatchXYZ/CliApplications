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
                ""Name"": ""PacktPublishing"",
                ""Url"": ""https://packpub.com/"",
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
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "PacktPublishing" && bookmark.Url == "https://packpub.com/"));
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "Audi Cars" && bookmark.Url == "https://www.audi.com/"));
        mockBookmarkService.Received(1).Import(Arg.Is<Bookmark>(bookmark => bookmark.Name == "LinkedIn" && bookmark.Url == "https://www.linkedin.com/"));
    }
}
