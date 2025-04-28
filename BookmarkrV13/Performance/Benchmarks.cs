using System.CommandLine;
using BenchmarkDotNet.Attributes;
using BookmarkrV13.Commands.Export;
using BookmarkrV13.Services.BookmarkService;

namespace BookmarkrV13.Performance;

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

    [Benchmark(Baseline = true)]
    public async Task ExportBookmarks()
    {
        var exportCmd = new ExportCommand(_bookmarkService!, "export", "Exports all bookmarks to a file");
        var exportArgs = new[]
        {
            "--file", "bookmarksbench.json"
        };

        await exportCmd.InvokeAsync(exportArgs);
    }

    [Benchmark]
    public async Task ExportBookmarksOptimized()
    {
        var exportCmd = new ExportCommandOptimized(_bookmarkService!, "export", "Exports all bookmarks to a file");
        var exportArgs = new[]
        {
            "--file", "bookmarksbench.json"
        };

        await exportCmd.InvokeAsync(exportArgs);
    }
}
