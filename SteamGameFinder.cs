using Microsoft.Win32;

namespace BonesClassLibrary.FileFinders;


/// <summary>
/// Finds the EXE of a Steam Game that is stored in the Common folder. Only for Windows 64 bit.
/// </summary>
public static class SteamGameFinder
{
    public static readonly string SteamCommonPath = Path.Combine(GetSteamPath(), @"steamapps\common");
    static readonly EnumerationOptions DefaultOption = new()
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = true,
    };

    /// <summary>
    /// exeName should be the file name without the .exe extension.
    /// </summary>
    public static string? Find(string exeName, EnumerationOptions? options = null)
    {
        return Directory.EnumerateFiles(SteamCommonPath, SearchPattern(exeName), options ?? DefaultOption).FirstOrDefault();
    }
    public static string FindOrThrow(string exeName, EnumerationOptions? options = null) => Find(exeName, options) ?? throw new FileNotFoundException(null, $"{exeName}.exe");

    static string SearchPattern(string exeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(exeName, nameof(exeName));
        return $"{exeName}*.exe";
    }
    static string GetSteamPath()
    {
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam")) //if 32 bit, must change this string
        {
            if (key?.GetValue("InstallPath") is string path) return path;
        }
        throw new DirectoryNotFoundException("Steam directory not found!");
    }
}
