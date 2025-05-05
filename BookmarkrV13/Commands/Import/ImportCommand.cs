using System.CommandLine;
using System.IO.Abstractions;
using System.Text.Json;
using BookmarkrV13.Models;
using BookmarkrV13.Services.BookmarkService;
using Serilog;

namespace BookmarkrV13.Commands.Import;

public class ImportCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;
    private readonly IFileSystem _fileSystem;

    #endregion

    #region Constructor

    public ImportCommand(
        IBookmarkService bookmarkService,
        string name,
        string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;
        _fileSystem = new FileSystem();

        AddOption(inputFileOption);
        this.SetHandler(OnImportCommand, inputFileOption);
    }

    internal ImportCommand(
        IBookmarkService bookmarkService,
        IFileSystem fileSystem,
        string name,
        string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;
        _fileSystem = fileSystem;

        AddOption(inputFileOption);
        this.SetHandler(OnImportCommand, inputFileOption);
    }

    #endregion

    #region Options

    private Option<FileInfo> inputFileOption = new Option<FileInfo>(
            ["--file", "-f"],
            "The input file that contains the bookmarks to be imported"
        )
        {
            IsRequired = true,
        }
        .LegalFileNamesOnly()
        .ExistingOnly();

    #endregion

    #region Handler method

    private void OnImportCommand(FileInfo inputFile)
    {
        var json = _fileSystem.File.ReadAllText(inputFile.FullName);
        var bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? [];

        foreach (var bookmark in bookmarks)
        {
            var conflict = _bookmarkService.Import(bookmark);
            if (conflict is not null)
            {
                Log.Information(
                    "{TimeStamp} | Bookmark updated | name changed from '{ConflictOldName}' to '{ConflictNewName}' for Url '{ConflictUrl}'",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    conflict.OldName,
                    conflict.NewName,
                    conflict.Url);
            }
        }
    }

    internal void OnImportCommand(IFileInfo fileInfo)
    {
        OnImportCommand(new FileInfo(fileInfo.FullName));
    }

    #endregion

}
