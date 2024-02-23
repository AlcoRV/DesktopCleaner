using DesktopCleaner.Interfaces;
using DesktopCleaner.Options;
using IWshRuntimeLibrary;
using Microsoft.Extensions.Configuration;

namespace DesktopCleaner.Services;

public class DesktopCleaner: IDesktopCleaner
{
    private readonly IgnoreItemsOptions _ignoreFilesOptions;

    public DesktopCleaner(IConfiguration configuration)
    {
        _ignoreFilesOptions = configuration.GetSection(IgnoreItemsOptions.Name).Get<IgnoreItemsOptions>()
            ?? new IgnoreItemsOptions();
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

        foldersToDeleting.ForEach(f => f.Delete(true));
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

        filesToDelete.Except(excessList)
            .ToList()
            .ForEach(f => f.Delete());
    }
}
