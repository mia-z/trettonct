using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrettonCodeTest.Enumerators;

namespace TrettonCodeTest.Models
{
    /// <summary>
    /// Class for holding information about a specific directory or file position
    /// Inherits from IEquatable for the purpose of hashing
    /// </summary>
    public class DirectoryIndex : IEquatable<DirectoryIndex>
    {
        /// <summary>
        /// The parent of this index, used to determining if the index is located at the root
        /// </summary>
        public DirectoryIndex? Parent { get; set; }
        /// <summary>
        /// The name associated with the index
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The depth of this index
        /// </summary>
        public int Depth { get; set; }
        /// <summary>
        /// Whether or not this index is explicitly declared as a root index
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        /// The type of index
        /// </summary>
        public IndexType IndexType { get; set; }
        /// <summary>
        /// Recursively fetches the full path of this index by traversing the parent paths until root is reached
        /// </summary>
        public string FullPath
        {
            get
            {
                if (Parent == null)
                    return Name;

                var pathLevels = new List<string>
                {
                    Name
                };

                var parentTraverse = Parent;
                while (parentTraverse != null)
                {
                    pathLevels.Add(parentTraverse.Name);
                    parentTraverse = parentTraverse.Parent;
                }

                pathLevels.Reverse();

                return string.Join("/", pathLevels);
            }
        }

        public DirectoryIndex(DirectoryIndex? parent, string name, int depth, bool isRoot, IndexType type)
        {
            Parent = parent;
            Name = name;
            Depth = depth;
            IsRoot = isRoot;
            IndexType = type;
        }

        /// <summary>
        /// Arbitrary Equals operator override
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>System.Boolean value determining the truthyness</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            
            var other = obj as DirectoryIndex;
            if (Name == other.Name && Depth == other.Depth) 
                return true;
            
            return false;
        }

        /// <summary>
        /// Generated Hash of this index based on it's full location and depth
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return FullPath.GetHashCode() ^ Depth;
        }

        /// <summary>
        /// Equals operator override to determine whether this DirectoryIndex is the same as another
        /// </summary>
        /// <param name="other">The DirectoryIndex to compare against</param>
        /// <returns>System.Boolean value determining the truthyness</returns>
        public bool Equals(DirectoryIndex? other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (Name == other.Name && Depth == other.Depth && IndexType == other.IndexType && Parent == other.Parent)
                return true;

            return false;
        }
    }
}
