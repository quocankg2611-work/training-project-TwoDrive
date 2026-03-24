namespace TwoDrive.Core.Helper;

public static class PathUtils
{
    public const string ROOT_PATH = "/";

    public static string[] SplitPath(string path)
    {
        return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    public static string CombinePath(params string[] parts)
    {
        return string.Join('/', parts);
    }

    public static string GetParentPath(string path)
    {
        var parts = SplitPath(path);
        if (parts.Length <= 1)
        {
            return ROOT_PATH; // No parent, return root
        }
        return CombinePath([.. parts.Take(parts.Length - 1)]);
    }

    public static string GetParentPathName(string path)
    {
        var par = SplitPath(path);
        if (par.Length == 0)
        {
            return ROOT_PATH;
        }
        return par.Last();
    }

    public static string ConcatPath(params string[] parts)
    {
        return string.Join('/', parts.Select(p => p.Trim('/')));
    }
}
