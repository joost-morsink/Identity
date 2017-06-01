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
        /// Constructs a new tree based on a collection of Paths.
        /// </summary>
        /// <param name="paths">The Paths to construct a tree for.</param>
        public PathMatchTree(IEnumerable<Path> paths)
        {
            _lookup = paths.Where(p => p.Count > 0).ToLookup(p => p[0]);
            _tree = new ConcurrentDictionary<string, PathMatchTree>();
            Terminals = paths.Where(p => p.Count == 0).ToArray();
        }
        private ConcurrentDictionary<string, PathMatchTree> _tree;
        private ILookup<string, Path> _lookup;

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
                if (_tree.TryGetValue(part, out var val))
                    return val;
                else
                {
                    var newVal = _lookup[part].Any() ? new PathMatchTree(_lookup[part].Select(path => path.Skip())) : null;
                    if (newVal == null)
                        return part == "*" ? null : this["*"];
                    else
                        return _tree.GetOrAdd(part, newVal);
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
