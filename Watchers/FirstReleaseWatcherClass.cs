namespace NugetHelpersLibrary.Watchers;
public class FirstReleaseWatcherClass
{
    public Action<string>? NewReleaseProcess { get; set; }
    private readonly FileSystemWatcher? _watches;
    private readonly string _sourcePath;
    private FileSystemWatcher? _finals;
    private string _name;
    public FirstReleaseWatcherClass(string sourcePath, EnumFirstWatchStatus status)
    {
        if (status == EnumFirstWatchStatus.Compiled)
        {
            throw new CustomBasicException("Compiled is not supported for the first watcher");
        }
        _name = $"{ff.FileName(sourcePath)}.dll";
        _sourcePath = sourcePath;
        if (status == EnumFirstWatchStatus.Beginning)
        {
            string binPath = Path.Combine(sourcePath, "bin");
            _watches = new(binPath);
            _watches.NotifyFilter = NotifyFilters.DirectoryName; //try only directory name (?)
            _watches.Renamed += _watches_Renamed;
            _watches.Created += _watches_Created;
            _watches.EnableRaisingEvents = true;
        }
        else
        {
            string path = fg.GetFirstDllPath(sourcePath);
            path = ff.GetParentPath(path);
            WaitForCompiled(path);
        }
        //Console.WriteLine(_name);
        
    }
    private void _watches_Created(object sender, FileSystemEventArgs e)
    {
        ProcessNewOrRenamed(e.Name!, e.FullPath);
    }
    private void _watches_Renamed(object sender, RenamedEventArgs e)
    {
        ProcessNewOrRenamed(e.Name!, e.FullPath);
    }
    private void ProcessNewOrRenamed(string name, string path)
    {
        if (NewReleaseProcess is null)
        {
            return; //because there is nothing.
        }
        if (name.EndsWith("Release"))
        {

            //Console.WriteLine("Release detected");
            //this is the first one.  means you only need to look out for the first folder created afterwards.
            BasicList<string> list = ff.DirectoryList(path);
            //Console.WriteLine(list.Count);
            string nextPart = Path.Combine(path, list.Single()); //should only have one if its a new project.
            //Console.WriteLine(nextPart);

            //Console.WriteLine("Counting files for release");

            //BasicList<string> files = ff.FileList(nextPart);
            //Console.WriteLine(files.Count);
            WaitForCompiled(nextPart);
           
            //this is for release
            
            //NewReleaseProcess.Invoke(_sourcePath);
        }
    }

    private void WaitForCompiled(string path)
    {
        _finals = new(path);
        _finals.NotifyFilter = NotifyFilters.FileName; //i think this alone.
        _finals.Created += _finals_Created;
        if (_watches is not null)
        {
            _watches.EnableRaisingEvents = false;
            _watches.Dispose();
        }
        Console.WriteLine("Waiting for dll to be compiled");
        _finals.EnableRaisingEvents = true;
    }

    private void _finals_Created(object sender, FileSystemEventArgs e)
    {
        if (NewReleaseProcess is null)
        {
            return;
        }
        if (e.Name!.ToLower() == _name.ToLower())
        {
            _finals!.EnableRaisingEvents = false;
            _finals.Dispose();
            NewReleaseProcess.Invoke(_sourcePath);
        }
    }
}