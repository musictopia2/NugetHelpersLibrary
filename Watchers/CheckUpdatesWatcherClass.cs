namespace NugetHelpersLibrary.Watchers;
public class CheckUpdatesWatcherClass
{
    public Action<string, DateTime>? NewUpdateDetected { get; set; }
    private readonly FileSystemWatcher _watcher;
    private readonly string _name;
    private readonly string _sourcePath;
    private DateTime _lastDate;
    public CheckUpdatesWatcherClass(string sourcePath, string currentnetVersion)
    {
        string path = cg.GetNewNetFirstDllPath(sourcePath, currentnetVersion);
        _name = ff.FullFile(path);
        path = ff.GetParentPath(path);
        _lastDate = DateTime.Now;
        _watcher = new(path);
        _watcher.NotifyFilter = NotifyFilters.LastWrite; //only write alone.
        _watcher.Changed += Watcher_Changed;
        _watcher.EnableRaisingEvents = true;
        _sourcePath = sourcePath;
    }
    bool CanUpdate()
    {
        if (_lastDate == default)
        {
            return true;
        }
        DateTime newdate = DateTime.Now;
        return newdate > _lastDate.AddSeconds(1);
    }
    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (CanUpdate() == false)
        {
            return;
        }
        if (NewUpdateDetected is null)
        {
            return;
        }
        if (_name.ToLower() != e.Name!.ToLower())
        {
            return;
        }
        //showed update 3 times (wrong).
        _lastDate = DateTime.Now;
        DateTime lastWrite = ff.GetFile(e.FullPath)!.DateModified;
        //for now, the source path is fine
        NewUpdateDetected.Invoke(_sourcePath, lastWrite);
    }
}