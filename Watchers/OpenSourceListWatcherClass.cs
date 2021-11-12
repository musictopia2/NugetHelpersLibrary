namespace NugetHelpersLibrary.Watchers;
public class OpenSourceListWatcherClass
{
    public Action<string>? NewOpenSourceProjectDetected { get; set; }
    public Action<string>? ErrorReported { get; set; }
    private readonly FileSystemWatcher _watcher;
    private readonly string _path;
    private BasicList<string> _list;
    public OpenSourceListWatcherClass(INugetSettings settings)
    {
        _path = settings.OpenSourcePath();
        _watcher = new(ff.GetParentPath(_path));
        _watcher.NotifyFilter = NotifyFilters.LastWrite;
        _watcher.Changed += _watcher_Changed;
        _watcher.EnableRaisingEvents = true;
        _list = ff.ReadAllLines(_path);
    }
    private async void _watcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (NewOpenSourceProjectDetected == null)
        {
            return;
        }
        if (e.FullPath == _path)
        {
            //what should come across is the new project path.
            BasicList<string> updated = await ff.ReadAllLinesAsync(_path);
            if (updated.Count == _list.Count)
            {
                return;
            }
            if (updated.Count < _list.Count)
            {
                _list = updated; //try to reset the list back again.  so if i add more, can still detect.  don't worry about deleted projects anymore.
                return;
            }
            if (updated.Count > _list.Count + 1)
            {
                ErrorReported?.Invoke("More than one project was added.  Only one project added at a time is supported");
                return;
            }
            string newProject = updated.Last();
            if (ff.DirectoryExists(newProject) == false)
            {
                ErrorReported?.Invoke("The new project detected does not exist");
                return;
            }
            string bins = Path.Combine(newProject, "bin");
            if (ff.DirectoryExists(bins) == false)
            {
                ErrorReported?.Invoke("The new project detected is not even a real visual studio project");
                return;
            }
            _list.Add(newProject);
            NewOpenSourceProjectDetected.Invoke(newProject);
        }
    }
}