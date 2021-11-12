namespace NugetHelpersLibrary.Models;
public enum EnumStatus
{
    //new or needcreate means has to create the nuget package.
    //the needstoupload means needs to actually upload the nuget package.

    None, New, NeedCreate, NeedsToUpload
}
