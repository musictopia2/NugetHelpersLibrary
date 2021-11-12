namespace NugetHelpersLibrary.Watchers;
public static class CompiledGlobalClass
{
    internal static string GetDllName(string sourcePath) => $"{ff.FileName(sourcePath)}.dll";
    internal static string GetNewNetFirstDllPath(string sourcePath, string version)
    {
        string name = GetDllName(sourcePath);
        string temps = Path.Combine(sourcePath, "bin", "Release", version, name);
        return temps;
    }
    public static DateTime GetLastWriteDate(string sourcePath, string version)
    {
        string realPath = GetNewNetFirstDllPath(sourcePath, version);
        return ff.GetFile(realPath)!.DateModified;
    }
    //if it detects new folder in release, needs to check the version.  if arm64 is used, ignore for example.
    public static string ProjectVersion(string sourcePath)
    {
        string name = GetDllName(sourcePath);
        string temps = Path.Combine(sourcePath, "bin", "Release");
        BasicList<string> list = ff.DirectoryList(temps);
        string lastVersion = "";
        list.ForEach(xx =>
        {
            string tempVersion = ff.FullFile(xx);
            string fins = GetNewNetFirstDllPath(sourcePath, tempVersion);
            if (ff.FileExists(fins))
            {
                lastVersion = tempVersion;
            }
        });
        if (lastVersion == "")
        {
            throw new CustomBasicException("No version was detected.  Should have checked for release first");
        }
        return lastVersion;
    }
    internal static BasicList<VersionModel> GetOutstandingNonCompiledVersions(string sourcePath, string version )
    {
        //has to have at least one version.  the version is guaranteed to not be on the list.
        if (version == "")
        {
            throw new CustomBasicException("Must have at least one good version");
        }
        //i don't think 
        string name = GetDllName(sourcePath);
        string temps = Path.Combine(sourcePath, "bin", "Release");
        BasicList<string> list = ff.DirectoryList(temps);
        BasicList<VersionModel> output = new();
        list.ForEach(xx =>
        {
            string tempVersion = ff.FullFile(xx);
            if (tempVersion != version)
            {
                string fins = GetNewNetFirstDllPath(sourcePath, tempVersion);
                if (ff.FileExists(fins) == false)
                {
                    output.Add(new(fins, name, tempVersion));
                }
            }
        });
        return output;
    }
}