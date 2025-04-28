using System.CommandLine;
using BenchmarkDotNet.Attributes;
using BookmarkrV12.Commands.Export;
using BookmarkrV12.Services.BookmarkService;

namespace BookmarkrV12.Performance;

[MemoryDiagnoser]
public class Benchmarks
{

    #region Properties

    private IBookmarkService? _bookmarkService;

    #endregion

    #region GlobalSetup

    [GlobalSetup]
    public void BenchmarksGlobalSetup()
    {
        _bookmarkService = new BookmarkService();
    }

    #endregion

    [Benchmark]
    public async Task ExportBookmarks()
    {
        var exportCmd = new ExportCommand(_bookmarkService!, "export", "Exports all bookmarks to a file");
        var exportArgs = new[]
        {
            "--file", "bookmarksbench.json"
        };

        await exportCmd.InvokeAsync(exportArgs);
    }
}
