namespace BonesClassLibrary.FileFinders;

public static class VietnamWarModLab
{
    public static readonly string Path = GetPath();
    static string GetPath()
    {
        string? path = Directory.EnumerateDirectories(KnownFolders.Desktop, "VietnamWarModLab", SearchOption.TopDirectoryOnly).FirstOrDefault();
        return path ?? throw new DirectoryNotFoundException("VietnamWarModLab not found on Desktop!");
    }
}