namespace DesktopCleaner.Options;

/// <summary>
///     Ignore settings for files and folders
/// </summary>
public class IgnoreItemsOptions
{
    public const string Name = "IgnoreFiles";

    public IEnumerable<string> Files { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Folders { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Extentions { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Prefixes { get; set; } = Array.Empty<string>();
    public IEnumerable<string> Sources { get; set; } = Array.Empty<string>();
}
