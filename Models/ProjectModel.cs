namespace NugetHelpersLibrary.Models;
public class ProjectModel
{
    //can cross reference with this if needed (?)
    //could be database as well.

    public string ProjectDirectory { get; set; } = "";
    public string CsPath { get; set; } = "";
    public string DllPath { get; set; } = "";
    public string NugetPath { get; set; } = "";
    public EnumStatus Status { get; set; }
    public DateTime LastModified { get; set; }
    public string LastPackageVersion { get; set; } = "";
    public string CurrentNetVersion { get; set; } = ""; //this is the version that is being used.
}