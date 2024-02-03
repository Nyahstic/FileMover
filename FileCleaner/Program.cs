using System.IO;
using Newtonsoft.Json;


namespace FileCleaner
{
    internal class Program
    {
        static private string watchingDirectory = "";
        static private string targetDirectory = "";

        static Dictionary<string, string> exttoDir = new Dictionary<string, string>();

        static int Main(string[] args)
        {
            if (!File.Exists("./config.cfg"))
            {
                Console.WriteLine("## THERE'S NO CONFIGURATION PRESENT! ##");
                Console.WriteLine("   Please configure the app to correctly sort the files.");

                File.WriteAllText(@"./config.cfg", "Rewrite this to be something like this:\n" +
                    "{\n" +
                    "\"source\":\"C:\\path\\to\\source\"," +
                    "\"target\":\"C:\\path\\to\\target\"," +
                    "\"txt\":\"\\placetostoreText\""+
                    "}");
 
                return -1;
            }



            var config = File.ReadAllText("./config.cfg");

            var config_JSON = JsonConvert.DeserializeObject<Dictionary<string,string>>(config);
            

            watchingDirectory = config_JSON["source"];
            targetDirectory = config_JSON["target"];

            foreach (var line in config_JSON)
            {
                Console.WriteLine(line.Key + "\t" + line.Value);
                if (line.Key == "source" || line.Key == "target") continue;
                try
                {
                    var val = line.Value.Replace("\\\\", "\\").Replace("\\", "");
                    Console.ForegroundColor = ConsoleColor.Magenta;

                    exttoDir.Add(line.Key, val);

                    Console.WriteLine($"The file extention {line.Key} will be moved to {(targetDirectory + '\\' + val)} .");
                    if (!Directory.Exists(targetDirectory + '\\' +  val))
                    {
                        Directory.CreateDirectory(targetDirectory + '\\' + val);
                        Console.WriteLine("Directory didn't exist, so I created one");
                    }
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }


            using var fsWatcher = new FileSystemWatcher(watchingDirectory);

            fsWatcher.NotifyFilter = NotifyFilters.DirectoryName
                                   | NotifyFilters.FileName; 

            fsWatcher.IncludeSubdirectories = true;
            fsWatcher.EnableRaisingEvents = true;

            fsWatcher.Created += OnFileCreation;
            fsWatcher.Error += OnException;

            while (true) ;
        }


        private static void OnFileCreation(object sender, FileSystemEventArgs e)
        {
            var ModifiedFile = e.FullPath;
            var FIleExt = Path.GetExtension(ModifiedFile).Remove(0, 1);
            var FileName = Path.GetFileName(ModifiedFile);
            if (exttoDir.ContainsKey(FIleExt))
            {
                var TargetPath = Path.Combine(targetDirectory, exttoDir[FIleExt]);
                var TargetFilePath = Path.Combine(TargetPath, FileName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{ModifiedFile} >>>> {TargetFilePath}");
                Console.ResetColor();
                try
                {
                    File.Copy(ModifiedFile, TargetFilePath, true);
                }
                catch (IOException ex)
                {
                    PrintException(ex);
                }
            }
            else Console.WriteLine($"Ignored {ModifiedFile}");
        }


        private static void OnException(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Got an Execption: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("\n\n");
                PrintException(ex.InnerException);
            }
        }

    }
}
