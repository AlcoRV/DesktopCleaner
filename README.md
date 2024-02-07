File DesktopCleaner/Properties/appsettings.json contains clean settings.
It's better to use in TaskSheduller.

Example:
{
  "IgnoreFiles": {
      "Files": [ "desktop.ini", "MSI Afterburner" ], // Delete all files, except these.
      "Folders": [ "AutoLogger", "AV_block_remover" ], // Delete all folders, except these.
      "Extentions": [], // Delete all files, except ones with these extentions (example, ".txt").
      "Prefixes": [ "i" ], // Delete all files and folders, except ones with these prefixes (example filename: "isomename.txt").
      "Sources": [ "Games" ] // Delete all shortcuts, except ones with these folders in sources.
}

P.s.: Don't delete "desktop.ini", "AutoLogger", "AV_block_remover" from settings.
