using DesktopCleaner.Properties.Settings;
using IWshRuntimeLibrary;
using Microsoft.Extensions.Configuration;

namespace DesktopCleaner.Services;

public class DesktopCleaner
{
    private readonly IgnoreItemsSettings _ignoreFilesSettings;

    public DesktopCleaner(IConfiguration configuration) {
        _ignoreFilesSettings = configuration.GetSection(IgnoreItemsSettings.Name).Get<IgnoreItemsSettings>()
            ?? new IgnoreItemsSettings();
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
            .Where(f => !_ignoreFilesSettings.Folders.Contains(f.Name))
            .Where(f => !_ignoreFilesSettings.Prefixes.Any(p => f.Name.StartsWith(p)))
            .ToList();

        foldersToDeleting.ForEach(f => f.Delete(true));
    }

    private void CleanFiles(IEnumerable<string> paths)
    {
        var filesToDelete = paths
            .SelectMany(p => new DirectoryInfo(p).GetFiles())
            .Where(f => !_ignoreFilesSettings.Files.Any(isf => f.Name.Contains(isf, StringComparison.OrdinalIgnoreCase)))
            .Where(f => !_ignoreFilesSettings.Prefixes.Any(p => f.Name.StartsWith(p)))
            .Where(f => !_ignoreFilesSettings.Extentions.Any(e => f.Extension.Equals(e, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        IWshShell shell = new WshShell();

        var excessList = filesToDelete
            .Where(f => f.Extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase))
            .Where(f => _ignoreFilesSettings.Sources.Any(s => (shell.CreateShortcut(f.FullName) as IWshShortcut)!.TargetPath.Contains(s, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        filesToDelete.Except(excessList)
            .ToList()
            .ForEach(f => f.Delete());
    }
}
