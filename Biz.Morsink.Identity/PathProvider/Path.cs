using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.PathProvider
{
    public struct Path
    {
        public static Path Parse(string pathString, Type data = null, char separator = '/')
            => new Path(pathString.Split(separator), data);
        public Path(IEnumerable<string> parts, Type data)
        {
            _parts = parts.ToArray();
            _skip = 0;
            Data = data;
        }
        private Path(string[] parts, int skip, Type data)
        {
            _parts = parts;
            _skip = skip;
            Data = data;
        }
        private string[] _parts;
        private int _skip;

        public int Count => _parts.Length - _skip;
        public int Arity => _parts.Where(p => p == "*").Count();

        public Type Data { get; }

        public string this[int index] => _parts[_skip + index];
        public Path Skip(int num = 1)
            => new Path(_parts, _skip + num, Data);
        public Match Match(Path other)
        {
            if (Count != other.Count)
                return default(Match);
            var result = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == "*")
                    result.Add(other[i]);
                else if (this[i] != other[i])
                    return default(Match);
            }
            return new Match(this, result.ToArray());
        }
        public Path GetFullPath()
            => new Path(_parts, 0, Data);
        private IEnumerable<string> fillHelper(IEnumerable<string> stars)
        {
            var s = stars.ToArray();
            int n = 0;
            for (int i = 0; i < _parts.Length; i++)
            {
                if (_parts[i] == "*")
                    yield return s[n++];
                else
                    yield return _parts[i];
            }
        }
        public Path FillStars(IEnumerable<string> stars)
            => new Path(fillHelper(stars), Data);

        public string PathString => string.Join("/", _parts);
    }
}
