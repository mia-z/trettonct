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
        private readonly List<string> _pathsFound;

        private PathMonitor()
        {
            _pathsFound = new List<string>();
        }

        public static readonly Lazy<PathMonitor> _instance = new Lazy<PathMonitor>(() => new PathMonitor());

        public static PathMonitor Instance => _instance.Value;

        public List<string> GetFoundPaths()
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
        }

        /// <summary>
        /// Add an array of paths to the list of found paths, via range
        /// </summary>
        /// <param name="path">System.String[] containing the multiple values found paths</param>
        public void AddPaths(string[] paths)
        {
            _pathsFound.AddRange(paths);
        }

        /// <summary>
        /// Add a list of paths to the list of found paths, via range
        /// </summary>
        /// <param name="path">List of System-String containing the multiple values found paths</param>
        public void AddPaths(List<string> paths)
        {
            _pathsFound.AddRange(paths);
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
    }
}