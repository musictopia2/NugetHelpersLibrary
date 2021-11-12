namespace NugetHelpersLibrary.MiscHelpers;
public static class GlobalHelpers
{
    public static string GetCsProject(string sourcePath)
    {
        string fullName = ff.FullFile(sourcePath);
        return Path.Combine(sourcePath, $"{fullName}.csproj");
    }
    public static string GetPackageVersionString(string source)
    {
        string path = GetCsProject(source);
        if (ff.FileExists(path) == false)
        {
            throw new CustomBasicException("No csproj file exist");
        }
        HtmlParser parses = new();
        string content = ff.AllText(path);
        parses.Body = content;
        return parses.GetSomeInfo("<Version>", "</Version>");
        //will need a way to match visual studios version (means if i manually update the version), will have that choice as well.
        //does automatically if changing .net versions (like if upgrading from 6 to future 7, etc).
        //i do reserve the right to manually update via visual studio the version because of making breaking changes even though the version did not change.
        //since i can detect, then if i change version manually, then will not increment but save on my side though.
    }
    public static int GetMajorVersion(string payLoad)
    {
        BasicList<string> output = payLoad.Split(".").ToBasicList();
        return int.Parse(output.First());
    }
    public static int GetMinorVersion(string payLoad)
    {
        BasicList<string> output = payLoad.Split(".").ToBasicList();
        return int.Parse(output.Last());
    }
    public static string IncrementMinorVersion(string payLoad)
    {
        int minor = GetMinorVersion(payLoad);
        int major = GetMajorVersion(payLoad);
        minor++;
        return $"{major}.0.{minor}";
    }
    public static string IncrementMajorVersion(string payLoad)
    {
        int major = GetMajorVersion(payLoad);
        major++;
        return $"{major}.0.1"; //start with one again.
    }
}