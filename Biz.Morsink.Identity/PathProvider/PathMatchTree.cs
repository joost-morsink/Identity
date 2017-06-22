using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    /// <summary>
    /// Utility for maintaining a lazy tree to match Paths.
    /// </summary>
    public class PathMatchTree
    {
        /// <summary>
        /// Creates a new case sensitive PathMatchTree 
        /// </summary>
        /// <param name="paths">The Paths to construct a tree for.</param>
        /// <returns>A new PathMatchTree instance</returns>
        public static PathMatchTree CaseSensitive(IEnumerable<Path> paths)
            => new PathMatchTree(null, paths);
        /// <summary>
        /// Creates a new case insensitive PathMatchTree 
        /// </summary>
        /// <param name="paths">The Paths to construct a tree for.</param>
        /// <returns>A new PathMatchTree instance</returns>
        public static PathMatchTree CaseInsensitive(IEnumerable<Path> paths)
            => new PathMatchTree(CaseInsensitiveEqualityComparer.Instance, paths);
        /// <summary>
        /// Constructs a new tree based on a collection of Paths.
        /// </summary>
        /// <param name="paths">The Paths to construct a tree for.</param>
        public PathMatchTree(IEqualityComparer<string> equalityComparer, IEnumerable<Path> paths)
        {
            equalityComparer = equalityComparer ?? EqualityComparer<string>.Default;
            lookup = paths.Where(p => p.Count > 0).ToLookup(p => p[0], equalityComparer);
            tree = new ConcurrentDictionary<string, PathMatchTree>(equalityComparer);
            this.equalityComparer = equalityComparer;
            Terminals = paths.Where(p => p.Count == 0).ToArray();
        }
        private ConcurrentDictionary<string, PathMatchTree> tree;
        private ILookup<string, Path> lookup;
        private readonly IEqualityComparer<string> equalityComparer;

        /// <summary>
        /// A collections of Paths that could exactly match the current position in the tree.
        /// </summary>
        public IReadOnlyList<Path> Terminals { get; }
    
        /// <summary>
        /// Traverses the tree down one level by using a part's content.
        /// </summary>
        /// <param name="part">The part to traverse.</param>
        /// <returns>a new PathMatchTree instance if there is a branch, null otherwise.</returns>
        public PathMatchTree this[string part]
        {
            get
            {
                if (tree.TryGetValue(part, out var val))
                    return val;
                else
                {
                    var newVal = lookup[part].Any() ? new PathMatchTree(equalityComparer, lookup[part].Select(path => path.Skip())) : null;
                    if (newVal == null)
                        return equalityComparer.Equals(part, "*") ? null : this["*"];
                    else
                        return tree.GetOrAdd(part, newVal);
                }
            }
        }
        /// <summary>
        /// Try matching a Path by walking the tree.
        /// </summary>
        /// <param name="path">The path to match.</param>
        /// <returns>A Match instance for the match result.</returns>
        public Match Walk(Path path)
        {
            if (path.Count == 0)
                return Terminals.Select(p => p.GetFullPath().Match(path.GetFullPath()))
                    .Where(m => m.IsSuccessful).FirstOrDefault();
            else
                return this[path[0]]?.Walk(path.Skip()) ?? default(Match);
        }

    }
}
