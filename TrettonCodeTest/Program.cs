
using System.Diagnostics;
using TrettonCodeTest.Enumerators;
using TrettonCodeTest.Models;
using TrettonCodeTest.Services;
/// <summary>
/// Author: Ryan Cockram
/// 
/// Code Test for Technical interview re. Tretton37.
/// 
/// Written in C#, using .NET 6
/// 
/// Repository: https://github.com/mia-z/trettonct
/// 
/// Task outline
/// Create a simple console program that can recursively traverse the Tretton37 root website directory
/// and save it to disk while keeping the integrity of the file structure.
/// 
/// </summary>
namespace TrettonCodeTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RCT37CT")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RCT37CT"));
            }
            
            Console.WriteLine($"Welcome to the Code Test for Tretton37 by Ryan Cockram\n" +
                $"This program will recursively download all the pages from https://tretton37.com/ and preserve their paths,\n" +
                $"saving the Html files on disk\n\n" +
                $"Hit Enter to begin!");

            Console.ReadLine();

            var fetchTimer = new Stopwatch();
            Console.SetCursorPosition(0, 6);
            Console.WriteLine($"Fetching all the pages under Https://tretton37.com");
            fetchTimer.Start();
            var rootPage = await new HtmlPage("").GetChildren();
            fetchTimer.Stop();

            Console.SetCursorPosition(0, 9);
            Console.WriteLine($"Finished fetching possible pages. Time taken: {fetchTimer.ElapsedMilliseconds}ms");

            Console.SetCursorPosition(0, 10);
            Console.WriteLine($"Indexing data and creating in-memory fs");
            var allPaths = rootPage.Children
                .UnionBy(rootPage.Children.SelectMany(x => x.Children), x => x)
                .ToHashSet();

            var vfs = Utils.CreateIndicies(allPaths.Select(x => x.Path).ToList());
            var directories = vfs.Where(x => x.IndexType == IndexType.DIRECTORY).ToList();

            Console.SetCursorPosition(0, 11);
            Console.WriteLine($"Writing HTML files to disk according to their structure");
            
            var writeTimer = new Stopwatch();
            writeTimer.Start();
            Utils.CreateDirectories(directories.Select(x => x.FullPath).ToList());
            List<Task> fswriteTasks = new List<Task>();
            foreach (var page in allPaths)
            {
                fswriteTasks.Add(Utils.WriteHtml(page.Path, page.RawHtml));
            }

            while (fswriteTasks.Any())
            {
                var finishedSearch = await Task.WhenAny(fswriteTasks);
                fswriteTasks.Remove(finishedSearch);
                PathMonitor.Instance.IncrementWriteProgress(allPaths.Count);
            }

            writeTimer.Stop();
            Console.SetCursorPosition(0, 13);
            Console.WriteLine($"Finished writing {allPaths.Count} files to disk. Time taken: {writeTimer.ElapsedMilliseconds}ms");
            Console.ReadLine();
        }
    }
}