using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrettonCodeTest.Enumerators;
using TrettonCodeTest.Models;

namespace TrettonCodeTest
{
    /// <summary>
    /// Basic static utility class containing helper functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Regular Expression pattern for matching text between anchor (<a></a>) tags of HTML
        /// Is very basic and doesn't cover every situation, but is fit for the current use case - probably shouldnt ever be used in production
        /// </summary>
        public static Regex AnchorPattern = new Regex(@"<a(.[^>]*)?>(\s*.[^<]*)<\/a>");

        /// <summary>
        /// Regular Expression pattern for matching text between the quotations of the string 'href=""' for the purpose of extracting link text contained within anchor tags
        /// Is also very basic and doesnt cover a lot of cases, but is suited for the current case - again, probably shouldnt ever use this in production
        /// </summary>
        public static Regex HrefPattern = new Regex("(?<=href=\\\"/)(.[^\\ ]*?)(?=\\\")");

        /// <summary>
        /// An arbitrary folder in a safe location where the files will be stored
        /// </summary>
        private static string dumpRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RCT37CT");

        /// <summary>
        /// Returns a list of anchor tag matches with the <see cref="AnchorPattern"/> Regex pattern</returns>
        /// </summary>
        /// <param name="html">The HTML string to search through</param>
        /// <returns>List of type System.String</returns>
        public static List<string> GetAnchors(string html) =>
            AnchorPattern.Matches(html).Select(x => x.Value).ToList();

        /// <summary>
        /// Extracts the text contained within the HREF property on anchor tags
        /// </summary>
        /// <param name="href">The HREF property to extract text from</param>
        /// <returns>System.String</returns>
        public static string GetHrefText(string href)
        {
            return HrefPattern.Match(href).Value; 
        }

        /// <summary>
        /// Creates a HashSet containing object IndexType for the purpose of soft-indexing the directories and files
        /// Creates a virtual file structure in memory which can be referenced later when saving files to disk
        /// Checks against 4 main values to ensure file system integrity similar to that of modern filesystems.
        /// - Verifies the parent of the Index - or not if it is null (root)
        /// - Verifies the depth 
        /// - Verifies the name
        /// - Verifies the index type - <see cref=">IndexType"/>
        /// Should all of these match, you can safely assume the files are attempting to be created in the same location 
        /// and thus cause a conflict, omitting the newest item(s)
        /// Checksum/Size/Meta-data would make this much more accurate, but isn't needed for this task.
        /// </summary>
        /// <param name="paths">List of System.String containing the paths to the files/directories</param>
        /// <returns>Hashed Set of Directory Index</returns>
        public static HashSet<DirectoryIndex> CreateIndicies(List<string?> paths)
        {
            HashSet<DirectoryIndex> directories = new HashSet<DirectoryIndex>();
            foreach (string path in paths)
            {
                var splitPaths = path.Split("/");
                for (var x = 0; x < splitPaths.Length; x++)
                {
                    if (x == 0)
                    {
                        if (splitPaths.Length == 1)
                        {
                            directories.Add(new DirectoryIndex(null, splitPaths[x], x, true, IndexType.FILE));
                        }
                        else
                        {
                            directories.Add(new DirectoryIndex(null, splitPaths[x], x, true, IndexType.DIRECTORY));
                        }
                    } else 
                    {
                        if (splitPaths[x].Split(".").Length > 1)
                        {
                            directories.Add(new DirectoryIndex(directories.Where(d => d.Name == splitPaths[x - 1]).First(), splitPaths[x], x, true, IndexType.FILE));
                        }
                        else
                        {
                            if (splitPaths.Length - 1 == x)
                            {
                                directories.Add(new DirectoryIndex(directories.Where(d => d.Name == splitPaths[x - 1]).First(), splitPaths[x], x, true, IndexType.FILE));
                            }
                            else
                            {
                                directories.Add(new DirectoryIndex(directories.Where(d => d.Name == splitPaths[x - 1]).First(), splitPaths[x], x, true, IndexType.DIRECTORY));
                            }
                        }
                    }
                }
            }
            return directories;
        }

        /// <summary>
        /// Helper function for creating the initial root directories
        /// Checks against the current einvornment if certain directories exist, and if not, creates them.
        /// </summary>
        /// <param name="paths">List of System.String paths to create directories for</param>
        public static void CreateDirectories(List<string> paths)
        {
            if (!Directory.Exists(dumpRoot))
            {
                Console.WriteLine("Creating non-existant root application folder");
                Directory.CreateDirectory(dumpRoot);
            }

            Console.WriteLine($"Creating {paths.Count} folders");
            foreach (string path in paths)
            {
                var thisDirectory = Path.Combine(dumpRoot, path);
                if (!Directory.Exists(thisDirectory))
                {
                    Directory.CreateDirectory(thisDirectory);
                    Console.WriteLine($"Created {path}");
                }
            }
        }

        /// <summary>
        /// Simple helper function that creates a scope-managed instance of StreamWriter for automatic disposal.
        /// Also appends files with .html in the event it doesn't exist. 
        /// </summary>
        /// <param name="path">The path where the file will be written</param>
        /// <param name="data">The HTML string of the webpage to be written to file</param>
        /// <returns></returns>
        public static async Task WriteHtml(string path, string data)
        {

            var filePath = Path.Combine(dumpRoot, path);
            if (!filePath.EndsWith(".html"))
            {
                filePath += ".html";
            }

            using (StreamWriter sw = new StreamWriter(filePath))
            {
                try
                {
                    await sw.WriteAsync(data);
                }
                catch (DirectoryNotFoundException dnf)
                {
                    Console.WriteLine("error writing file", dnf);
                }
            }
        }
    }
}
