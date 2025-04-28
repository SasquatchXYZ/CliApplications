using BookmarkrV12.Commands.Link;
using BookmarkrV12.Commands.Link.Add;
using BookmarkrV12.Services.BookmarkService;
using NSubstitute;

namespace Bookmarkr.UnitTests.Commands.Link;

[TestClass]
public class LinkCommandTests
{
    [TestMethod]
    public void LinkCommand_CallingClassConstructor_EnsuresThatLinkAddCommandIsTheOnlySubCommandOfLinkCommand()
    {
        // Arrange
        var bookmarkService = Substitute.For<IBookmarkService>();
        var expectedSubCommand = new LinkAddCommand(bookmarkService, "add", "Add a new bookmark link");

        // Act
        var actualCommand = new LinkCommand(bookmarkService, "link", "Manage bookmark links");
        var actualSubCommand = actualCommand.Subcommands[0];

        // Assert
        Assert.AreEqual(1, actualCommand.Subcommands.Count);
        CollectionAssert.AreEqual(actualSubCommand.Aliases.ToList(), expectedSubCommand.Aliases.ToList());
        Assert.AreEqual(actualSubCommand.Description, expectedSubCommand.Description);
    }

}
