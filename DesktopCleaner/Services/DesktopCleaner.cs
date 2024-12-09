using DesktopCleaner.Interfaces;
using DesktopCleaner.Options;
using IWshRuntimeLibrary;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DesktopCleaner.Services;

public class DesktopCleaner: IDesktopCleaner
{
    private readonly IgnoreItemsOptions _ignoreFilesOptions;
    private readonly ILogger<DesktopCleaner> _logger;

    public DesktopCleaner(IConfiguration configuration, ILogger<DesktopCleaner> logger)
    {
        _ignoreFilesOptions = configuration.GetSection(IgnoreItemsOptions.Name).Get<IgnoreItemsOptions>()
            ?? new IgnoreItemsOptions();

        _logger = logger;
    }

    public void Clean()
    {
        var paths = new List<string>() {
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };

        CleanFiles(paths);
        CleanFolders(paths);
    }

    private void CleanFolders(IEnumerable<string> paths)
    {
        var foldersToDeleting = paths
            .SelectMany(p => new DirectoryInfo(p).GetDirectories())
            .Where(f => !_ignoreFilesOptions.Folders.Contains(f.Name))
            .Where(f => !_ignoreFilesOptions.Prefixes.Any(p => f.Name.StartsWith(p)))
            .ToList();

        _logger.LogDebug("Folders: {folders}", string.Join(", ", foldersToDeleting.Select(f => f.Name)));

        foreach (var f in foldersToDeleting)
        {
            f.Delete(true);
            _logger.LogInformation("{f} deleted", f.Name);
        }
    }

    private void CleanFiles(IEnumerable<string> paths)
    {
        var filesToDelete = paths
            .SelectMany(p => new DirectoryInfo(p).GetFiles())
            .Where(f => !_ignoreFilesOptions.Files.Any(isf => f.Name.Contains(isf, StringComparison.OrdinalIgnoreCase)))
            .Where(f => !_ignoreFilesOptions.Prefixes.Any(p => f.Name.StartsWith(p)))
            .Where(f => !_ignoreFilesOptions.Extentions.Any(e => f.Extension.Equals(e, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        IWshShell shell = new WshShell();

        var excessList = filesToDelete
            .Where(f => f.Extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            .Where(f => _ignoreFilesOptions.Sources.Any(s => (shell.CreateShortcut(f.FullName) as IWshShortcut)!.TargetPath.Contains(s, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        filesToDelete = filesToDelete.Except(excessList).ToList();

        _logger.LogDebug("Files: {files}", string.Join(", ", filesToDelete.Select(f => f.Name)));

        foreach (var f in filesToDelete)
        {
            f.Delete();
            _logger.LogInformation("{f} deleted", f.Name);
        }
    }
}
