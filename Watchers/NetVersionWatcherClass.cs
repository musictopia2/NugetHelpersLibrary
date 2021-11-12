namespace NugetHelpersLibrary.Watchers;
public class NetVersionWatcherClass
{
    public Action<string, string>? NewNetVersionDetected { get; set; }
    private BasicList<VersionModel> _outstandingList = new();
    private readonly string _sourcePath;
    private readonly FileSystemWatcher _currentWatch;
    public NetVersionWatcherClass(string sourcePath, string currentVersion)
    {
        //Console.WriteLine($"Current version for {sourcePath} is {currentVersion}");
        _outstandingList = cg.GetOutstandingNonCompiledVersions(sourcePath, currentVersion);
        //Console.WriteLine($"{_outstandingList.Count} outstanding uncompiled net versions");
        _outstandingList.ForEach(item =>
        {
            AddWatcher(item);
        });
        _sourcePath = sourcePath;
        string path = Path.Combine(_sourcePath, "bin", "Release");
        _currentWatch = new(path); //detect folders.
        _currentWatch.NotifyFilter = NotifyFilters.DirectoryName;
        _currentWatch.Created += CurrentCreated;
        _currentWatch.Renamed += CurrentRenamed;
        _currentWatch.EnableRaisingEvents = true;
    }
    private void AddWatcher(VersionModel item)
    {
        string path = item.Path.ToLower().EndsWith("dll") ? ff.GetParentPath(item.Path) : item.Path;
        FileSystemWatcher watches = new(path);
        watches.NotifyFilter = NotifyFilters.FileName;
        watches.Created += Watches_Created;
        watches.EnableRaisingEvents = true;
    }
    private void CurrentRenamed(object sender, RenamedEventArgs e)
    {
        CurrentChanged(e.FullPath, e.Name!);
    }
    private void CurrentCreated(object sender, FileSystemEventArgs e)
    {
        CurrentChanged(e.FullPath, e.Name!);
    }
    private void CurrentChanged(string path, string name)
    {
        if (name.StartsWith("net") == false)
        {
            return; //has to start with net
        }
        VersionModel model = new(path, cg.GetDllName(_sourcePath), name);
        _outstandingList.Add(model);
        Console.WriteLine("Adding To Watcher");
        AddWatcher(model);

    }
    private void Watches_Created(object sender, FileSystemEventArgs e)
    {
        if (NewNetVersionDetected is null)
        {
            return;
        }
        VersionModel? version = _outstandingList.SingleOrDefault(xx => xx.Name.ToLower() == e.Name!.ToLower());
        if (version is not null)
        {
            FileSystemWatcher? watches = sender as FileSystemWatcher;
            watches!.EnableRaisingEvents = false;
            watches.Dispose();
            NewNetVersionDetected.Invoke(_sourcePath, version.Version);
        }
    }
}