using static BookmarkrV9.Utilities.Helper;
using System.CommandLine;
using System.Text.Json;
using BookmarkrV9.Services.BookmarkService;

namespace BookmarkrV9.Commands.Export;

public class ExportCommand : Command
{

    #region Properties

    private readonly IBookmarkService _bookmarkService;

    #endregion

    #region Constructor

    public ExportCommand(IBookmarkService bookmarkService, string name, string? description = null) : base(name, description)
    {
        _bookmarkService = bookmarkService;

        AddOption(outputFileOption);
        this.SetHandler(async context =>
        {
            var outputFileOptionValue = context.ParseResult.GetValueForOption(outputFileOption);
            var cancellationToken = context.GetCancellationToken();
            await OnExportCommand(outputFileOptionValue, cancellationToken);
        });
    }

    #endregion

    #region Options

    private Option<FileInfo> outputFileOption = new Option<FileInfo>(
        ["--file", "-f"],
        "The output file that will store the bookmarks"
    )
    {
        IsRequired = true,
    }.LegalFileNamesOnly();

    #endregion

    #region Handler method

    private async Task OnExportCommand(FileInfo outputFile, CancellationToken cancellationToken)
    {
        try
        {
            var bookmarks = _bookmarkService.GetAll();
            var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(outputFile.FullName, json, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            var requested = ex.CancellationToken.IsCancellationRequested
                ? "Cancellation was requested by you."
                : "Cancellation was NOT requested by you.";

            ShowWarningMessage(["Operation was cancelled.", requested, $"Cancellation reason: {ex.Message}"]);
        }
        catch (JsonException ex)
        {
            ShowErrorMessage([$"Failed to serialize bookmarks to JSON.", $"Error message {ex.Message}"]);
        }
        catch (UnauthorizedAccessException ex)
        {
            ShowErrorMessage([$"Insufficient permissions to access the file {outputFile.FullName}.", $"Error message {ex.Message}"]);
        }
        catch (DirectoryNotFoundException ex)
        {
            ShowErrorMessage([$"The file {outputFile.FullName} cannot be found due to an invalid path.", $"Error message {ex.Message}"]);
        }
        catch (PathTooLongException ex)
        {
            ShowErrorMessage([$"The provided path is exceeding the maximum length.", $"Error message {ex.Message}"]);
        }
        catch (Exception ex)
        {
            ShowErrorMessage([$"An unknown exception occurred.", $"Error message {ex.Message}"]);
        }
    }

    #endregion

}
