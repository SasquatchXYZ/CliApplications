using BookmarkrV9.Commands.Link;
using BookmarkrV9.Commands.Link.Add;
using BookmarkrV9.Services.BookmarkService;

namespace Bookmarkr.UnitTests.Commands.Link;

[TestClass]
public class LinkCommandTests
{
    [TestMethod]
    public void LinkCommand_CallingClassConstructor_EnsuresThatLinkAddCommandIsTheOnlySubCommandOfLinkCommand()
    {
        // Arrange
        IBookmarkService service = null;
        var expectedSubCommand = new LinkAddCommand(service, "add", "Add a new bookmark link");

        // Act
        var actualCommand = new LinkCommand(service, "link", "Mange bookmark links");
        var actualSubCommand = actualCommand.Subcommands[0];

        // Assert
        Assert.AreEqual(1, actualCommand.Subcommands.Count);
        CollectionAssert.AreEqual(actualSubCommand.Aliases.ToList(), expectedSubCommand.Aliases.ToList());
        Assert.AreEqual(actualSubCommand.Description, expectedSubCommand.Description);
    }

}
