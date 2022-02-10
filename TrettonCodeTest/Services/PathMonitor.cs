using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrettonCodeTest.Services
{
    /// <summary>
    /// Basic Singleton pattern for creating a service that will monitor the paths used to prevent infinite recursion
    /// While there are multiple different ways to implement a singleton pattern(block checking, srp prevention), this simple one will suffice for the task.
    /// </summary>
    public sealed class PathMonitor
    {
        private readonly HashSet<string> _pathsFound;
        private int _currentTaskCount;
        private int _totalTaskCount;

        private PathMonitor()
        {
            _pathsFound = new HashSet<string>();
            _currentTaskCount = 0;
            _totalTaskCount = 0;
        }

        public static readonly Lazy<PathMonitor> _instance = new Lazy<PathMonitor>(() => new PathMonitor());

        public static PathMonitor Instance => _instance.Value;

        public HashSet<string> GetFoundPaths()
        {
            return _pathsFound;
        }

        /// <summary>
        /// Adds a path to the list of found paths
        /// </summary>
        /// <param name="path">System.String containing the value of the found path</param>
        public void AddPath(string path)
        {
            _pathsFound.Add(path);

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.Write($"Found {_pathsFound.Count} unique pages");
        }

        /// <summary>
        /// Checks to see if a path has already been found
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>System.Boolean denoting whether the path was found (true) or not (false)</returns>
        public bool PathExists(string path)
        {
            return _pathsFound.Contains(path);
        }

        public void IncrementTaskCount(int amount)
        {
            _currentTaskCount += amount;
            _totalTaskCount += amount;

            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.Write($"Current # of tasks running: {_currentTaskCount}");

            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.Write($"Total tasks completed: {_totalTaskCount}");
        }

        public void DecrementTaskCount()
        {
            _currentTaskCount--;

            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
            Console.Write($"Current # of tasks running: {_currentTaskCount}");
        }
    }
}