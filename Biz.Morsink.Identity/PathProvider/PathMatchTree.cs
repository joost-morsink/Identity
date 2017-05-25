using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    public class PathMatchTree
    {
        public PathMatchTree(IEnumerable<Path> paths)
        {
            _lookup = paths.Where(p => p.Count > 0).ToLookup(p => p[0]);
            _tree = new ConcurrentDictionary<string, PathMatchTree>();
            Terminals = paths.Where(p => p.Count == 0).ToArray();
        }
        private ConcurrentDictionary<string, PathMatchTree> _tree;
        private ILookup<string, Path> _lookup;

        public IReadOnlyList<Path> Terminals { get; }
        //public PathMatchTree this[string part] => _tree.GetOrAdd(part, p => _lookup[p].Any() ? new PathMatchTree(_lookup[p].Select(path => path.Skip())) : null) ?? (part != "*" ? this["*"] : null);
    
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
                    {
                        _tree.TryAdd(part, newVal);
                        return newVal;
                    }
                }
            }
        }
        public Match Walk(Path path)
        {
            if (path.Count == 0)
                return Terminals.Select(p => p.GetFullPath().Match(path.GetFullPath()))
                    .Where(m => m.IsSuccess).FirstOrDefault();
            else
                return this[path[0]]?.Walk(path.Skip()) ?? default(Match);
        }

    }
}
