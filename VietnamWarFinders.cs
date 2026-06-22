namespace BonesClassLibrary.FileFinders;

/// <summary>
/// Folder directory for the Vietnam War Mod Lab.
/// </summary>
public static class VietnamWarModLab
{
    public static readonly string Path = GetPath();
    static string GetPath()
    {
        string? path = Directory.EnumerateDirectories(KnownFolders.Desktop, "VietnamWarModLab", SearchOption.TopDirectoryOnly).FirstOrDefault();
        return path ?? throw new DirectoryNotFoundException("VietnamWarModLab not found on Desktop!");
    }
}

/// <summary>
/// Folder directory for the Vietnam War game.
/// </summary>
public static class VietnamWarSource
{
    public static readonly string Path = System.IO.Path.GetDirectoryName(SteamGameFinder.FindOrThrow("Vietnam War"))!;
}