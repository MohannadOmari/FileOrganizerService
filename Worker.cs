namespace FileOrganizerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public static string source = @"C:\Users\mohan\Desktop\organizer";
        public static string destination = @"C:\Users\mohan\Desktop\movedTo";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var watcher = new FileSystemWatcher(@"C:\Users\mohan\Desktop\organizer", "*.txt");

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                /*watcher.Deleted += OnDeleted;
                watcher.Renamed += OnRenamed;*/
                watcher.Error += OnError;

                /*watcher.Filter = "*.png";*/
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Service started");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            /*string destinationPath = @"C:\Users\mohan\Desktop\movedTo";
            System.IO.File.Move($@"{e.FullPath}", $@"{destinationPath}\{e.Name}");*/
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            DateTime fileDate = System.IO.File.GetCreationTime(e.FullPath);
            if (!Directory.Exists($@"C:\Users\mohan\Desktop\movedTo\{fileDate.Year}"))
            {
                Directory.CreateDirectory($@"C:\Users\mohan\Desktop\movedTo\{fileDate.Year}");
            }
            if (!Directory.Exists($@"C:\Users\mohan\Desktop\movedTo\{fileDate.Year}\{fileDate.Month}"))
            {
                Directory.CreateDirectory($@"C:\Users\mohan\Desktop\movedTo\{fileDate.Year}\{fileDate.Month}");
            }
            System.IO.File.Move($@"{e.FullPath}", $@"{destination}\{fileDate.Year}\{fileDate.Month}\{e.Name}");
            Console.WriteLine($@"Moved to: {destination}\{fileDate.Year}\{fileDate.Month}");
            /*Console.WriteLine($"Name: {e.Name}");
            Console.WriteLine($"Month: {fileDate.Month}\nYear: {fileDate.Year}");
            Console.WriteLine(value);*/
        }

        /*private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");*/

        /*private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }*/

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}