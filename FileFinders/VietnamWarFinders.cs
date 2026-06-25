namespace BonesClassLibrary.FileFinders;

/// <summary>
/// Folder directory for the Vietnam War Mod Lab.
/// </summary>
public static class VietnamWarModLab
{
    public static readonly string Path = Directory.EnumerateDirectories(KnownFolders.Desktop, "VietnamWarModLab", SearchOption.TopDirectoryOnly).FirstOrDefault() 
    ?? throw new DirectoryNotFoundException("VietnamWarModLab not found on Desktop!");

    //could cause problems if a program exe is not located in vietnamwarmodlab, but all the related programs to vietnamwar are located there
    static string BackupFinder()
    {
        string dir = Directory.GetCurrentDirectory();
        string name = System.IO.Path.GetFileName(dir);
        while(name != "VietnamWarModLab")
        {
            dir = System.IO.Path.GetDirectoryName(dir)!;
            name = System.IO.Path.GetFileName(dir);
        }
        return dir;
    }
}
/// <summary>
/// Folder directory for the Vietnam War game.
/// </summary>
public static class VietnamWarSource
{
    public static readonly string Path = System.IO.Path.GetDirectoryName(SteamGameFinder.FindOrThrow("Vietnam War"))!;


}