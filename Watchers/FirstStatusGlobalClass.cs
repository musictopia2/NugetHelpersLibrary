namespace NugetHelpersLibrary.Watchers;
public static class FirstStatusGlobalClass
{
    internal static string GetFirstDllPath(string sourcePath)
    {
        string name = $"{ff.FileName(sourcePath)}.dll";
        string temps = Path.Combine(sourcePath, "bin", "Release");
        BasicList<string> list = ff.DirectoryList(temps);
        string fins = Path.Combine(temps, list.Single(), name);
        return fins;
    }
    

    //public static string GetWatchedForFirstRelease(this string path) => Path.Combine(path, "bin", "Release");
    public static EnumFirstWatchStatus GetFirstWatchStatus(string path)
    {
        if (ff.DirectoryExists(path) == false)
        {
            throw new CustomBasicException("Path for the watcher status does not even exist");
        }
        string temps = Path.Combine(path, "bin");
        if (ff.DirectoryExists(temps) == false)
        {
            throw new CustomBasicException("Bin folder was not created for the watcher status");
        }
        temps = Path.Combine(temps, "Release");
        if (ff.DirectoryExists(temps) == false)
        {
            return EnumFirstWatchStatus.Beginning;
        }
        //still beginning if there is a release but no dll.
        BasicList<string> list = ff.DirectoryList(temps);
        if (list.Count > 1)
        {
            return EnumFirstWatchStatus.Compiled; //because will assume if there is more than one version, then show as compiled.
        }
        string fins = GetFirstDllPath(path);
        if (ff.FileExists(fins) == false)
        {
            return EnumFirstWatchStatus.HadErrors;
        }
        return EnumFirstWatchStatus.Compiled;

        //return EnumFirstWatchStatus.Compiled;
    }
}