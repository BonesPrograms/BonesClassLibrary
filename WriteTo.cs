namespace BonesClassLibrary.IO;

public static class Write
{

    public static void To(string path, object obj, bool append = true)
    {
        To(path, obj.ToString(), append);
    }
    public static void To(string path, string? message, bool append = true)
    {
        SafetyCheck(path);
        using StreamWriter writer = new(path, append);
        writer.WriteLine(message);

    }
    public static void To<T>(string path, IList<T> objs, Func<T, string>? expr = null, bool append = true, bool writeLine = true)
    {
        SafetyCheck(path);
        using StreamWriter writer = new(path, append);
        foreach (var obj in objs)
        {
            string? write;
            if (expr != null)
                write = expr(obj);
            else
                write = obj?.ToString();
            if (writeLine)
                writer.WriteLine(write);
            else
                writer.Write(write);
        }
    }
    static void SafetyCheck(string path)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(path);
        string? dir = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(dir);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!File.Exists(path))
        {
            using FileStream fs = File.Create(path);
        }

    }
}