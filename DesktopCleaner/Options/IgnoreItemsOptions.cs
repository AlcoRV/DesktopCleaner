namespace DesktopCleaner.Options;

/// <summary>
///     Ignore settings for files and folders
/// </summary>
public class IgnoreItemsOptions
{
    public const string Name = "IgnoreFiles";

    public IEnumerable<string> Files { get; set; } = [];
    public IEnumerable<string> Folders { get; set; } = [];
    public IEnumerable<string> Extentions { get; set; } = [];
    public IEnumerable<string> Prefixes { get; set; } = [];
    public IEnumerable<string> Sources { get; set; } = [];
}
